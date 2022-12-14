using OpenTibiaUnity.Core.Input.Mapping;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityInput = UnityEngine.Input;

namespace OpenTibiaUnity.Core.Input
{
    public class InputHandler
    {
        public class ModifierKeyEvent : Utils.EventImpl<char, KeyCode, EventModifiers> { }
        public class KeyboardEvent : Utils.EventImpl<Event, bool> { }
        public class MouseEvent : Utils.EventImpl<Event, MouseButton, bool> { }

        private const int KeyRepeatMinDelay = 250;

        private bool _captureKeyboard = true;
        private bool _captureMouse = true;
        private bool _keyboardHandlerActive = false;
        private bool _mouseHandlerActive = false;
        private bool _ignoreNextLeft = false;
        private bool _ignoreNextRight = false;
        private bool _numlock = false;
        private KeyCode _keyCode = KeyCode.None;

        private Mapping.Mapping _mapping;

        private List<Binding> _movementBindings = null;
        private int[] _keyPressed = new int[500];
        private int[] _mousePressed = new int[3];
        private int[] _mouseDragged = new int[3];

        public ModifierKeyEvent onModifierKeyEvent = new ModifierKeyEvent();
        private KeyboardEvent onKeyDownEvent = new KeyboardEvent();
        private KeyboardEvent onKeyUpEvent = new KeyboardEvent();
        private MouseEvent onMouseDownEvent = new MouseEvent();
        private MouseEvent onMouseUpEvent = new MouseEvent();
        private MouseEvent onDragEvent = new MouseEvent();
        private MouseEvent onBeginDragEvent = new MouseEvent();
        private MouseEvent onEndDragEvent = new MouseEvent();

        public InputHandler() {
            OpenTibiaUnity.GameManager.AddSecondaryTimerListener(OnKeyboardRepeatTimer);
            OpenTibiaUnity.GameManager.AddSecondaryTimerListener(OnMouseRepeatTimer);
        }

        public void Cleanup() {
            OpenTibiaUnity.GameManager.RemoveSecondaryTimerListener(OnKeyboardRepeatTimer);
            OpenTibiaUnity.GameManager.RemoveSecondaryTimerListener(OnMouseRepeatTimer);
        }

        public void UpdateMapping() {
            var optionStorage = OpenTibiaUnity.OptionStorage;

            Mapping.Mapping mapping = null;
            MappingSet mappingSet = optionStorage.GetMappingSet(optionStorage.GeneralInputSetId);
            if (mappingSet != null) {
                if (optionStorage.GeneralInputSetMode == MappingSet.ChatModeOFF)
                    mapping = mappingSet.ChatModeOffMapping;
                else
                    mapping = mappingSet.ChatModeOnMapping;
            }
            
            if (mapping != null && mapping.Bindings != null) {
                _movementBindings = new List<Binding>();
                foreach (var binding in mapping.Bindings) {
                    if (binding.Action != null && (binding.Action as StaticAction.PlayerMove) != null)
                        _movementBindings.Add(binding);
                }
            }

            _mapping = mapping;
        }

        public void OnKeyboardRepeatTimer() {
            // Currently this is only useful for movement //
            if (_captureKeyboard && _movementBindings != null && !OpenTibiaUnity.GameManager.ActiveBlocker.gameObject.activeSelf) {
                var ticks = OpenTibiaUnity.TicksMillis;

                EventModifiers modifier = EventModifiers.None;
                if (IsKeyPressed(KeyCode.LeftAlt) || IsKeyPressed(KeyCode.RightAlt)) modifier |= EventModifiers.Alt;
                if (IsKeyPressed(KeyCode.LeftControl) || IsKeyPressed(KeyCode.RightControl)) modifier |= EventModifiers.Control;
                if (IsKeyPressed(KeyCode.LeftShift) || IsKeyPressed(KeyCode.RightShift)) modifier |= EventModifiers.Shift;

                bool isBlockerActive = OpenTibiaUnity.GameManager.ActiveBlocker.gameObject.activeSelf;
                foreach (var binding in _movementBindings) {
                    if (IsKeyPressed(binding.KeyCode) && _keyPressed[(int)binding.KeyCode] + KeyRepeatMinDelay < ticks
                        && binding.AppliesTo((binding.Action as StaticAction.PlayerMove).EventMask, binding.KeyCode, modifier, isBlockerActive)) {
                        binding.Action.Perform(true);
                        break;
                    }
                }
            }
        }

        public void OnMouseRepeatTimer() {
            // Do something when mouse is held
        }

        public void ClearPressedKeys() {
            _keyPressed = new int[500];
        }

        public bool IsKeyPressed(KeyCode keyCode) {
            return _keyPressed[(int)keyCode] != 0;
        }

        public bool IsMouseButtonPressed(MouseButton mouseButton) {
            switch (mouseButton) {
                case MouseButton.Left: return _mousePressed[0] != 0;
                case MouseButton.Right: return _mousePressed[1] != 0;
                case MouseButton.Middle: return _mousePressed[2] != 0;
                case MouseButton.Both: return _mousePressed[0] != 0 && _mousePressed[1] != 0;
            }
            return false;
        }

        public bool IsAnyMousePressed() {
            return IsMouseButtonPressed(MouseButton.Left)
                || IsMouseButtonPressed(MouseButton.Right)
                || IsMouseButtonPressed(MouseButton.Middle)
                || IsMouseButtonPressed(MouseButton.Both);
        }

        public bool IsMouseButtonDragged(MouseButton mouseButton) {
            switch (mouseButton) {
                case MouseButton.Left: return _mouseDragged[0] != 0;
                case MouseButton.Right: return _mouseDragged[1] != 0;
                case MouseButton.Middle: return _mouseDragged[2] != 0;
                case MouseButton.Both: return _mouseDragged[0] != 0 && _mouseDragged[1] != 0;
            }
            return false;
        }

        public void OnKeyEvent(Event e) {
            if (_keyboardHandlerActive || (int)e.keyCode >= _keyPressed.Length)
                return;
            
            try {
                _keyboardHandlerActive = true;
                var keyModified = false;
                if (e.type == EventType.KeyUp) {
                    keyModified = _keyPressed[(int)e.keyCode] != 0;
                    _keyPressed[(int)e.keyCode] = 0;

                    bool numlock = (e.modifiers & EventModifiers.Numeric) != 0;
                    if (numlock != _numlock) {
                        ClearPressedKeys();
                        _numlock = numlock;
                    }
                }

                if (_captureKeyboard && _mapping != null && !(e.alt && e.control)) {
                    if (e.type == EventType.KeyDown) {
                        var type = _keyPressed[(int)e.keyCode] != 0 ? InputEvent.KeyRepeat : InputEvent.KeyDown;
                        bool handled = _mapping.OnKeyInput(type, e.character, e.keyCode, e.modifiers);
                        if (!handled)
                            onKeyDownEvent.InvokeWhile(e, _keyPressed[(int)e.keyCode] != 0, () => e.type != EventType.Used);
                        else
                            e.Use();
                    } else if (e.type == EventType.KeyUp) {
                        bool handled = _mapping.OnKeyInput(InputEvent.KeyUp, e.character, e.keyCode, e.modifiers);
                        if (!handled)
                            onKeyUpEvent.InvokeWhile(e, false, () => e.type != EventType.Used);
                        else
                            e.Use();
                    }
                }

                if (e.type == EventType.KeyDown) {
                    _keyCode = e.keyCode;
                    keyModified = _keyPressed[(int)e.keyCode] != 0;
                    _keyPressed[(int)e.keyCode] = OpenTibiaUnity.TicksMillis;
                }

                if (keyModified)
                    onModifierKeyEvent.Invoke(e.character, e.keyCode, e.modifiers);
            } catch (System.Exception) {
            } finally {
                _keyboardHandlerActive = false;
            }
        }

        public void OnMouseEvent(Event e) {
            if (_mouseHandlerActive || e.button > _mousePressed.Length)
                return;

            try {
                _mouseHandlerActive = true;
                var eventType = e.type;
                var rawMouseButton = e.button;
                if (eventType == EventType.MouseUp)
                    _mousePressed[e.button] = 0;

                if (_captureMouse && !(e.alt && e.control) && e.button != 2) {
                    if (eventType == EventType.MouseDown) {
                        bool leftPressedEarlier = _mousePressed[0] != 0;
                        bool rightPressedEarlier = _mousePressed[1] != 0;

                        bool both;
                        if (e.button == 0)
                            both = rightPressedEarlier;
                        else
                            both = leftPressedEarlier;

                        if (both) {
                            _ignoreNextLeft = _ignoreNextRight = false;
                            onMouseDownEvent.Invoke(e, MouseButton.Both, leftPressedEarlier && rightPressedEarlier);
                        } else if (e.button == 0) {
                            onMouseDownEvent.Invoke(e, MouseButton.Left, leftPressedEarlier);
                        } else if (e.button == 1) {
                            onMouseDownEvent.Invoke(e, MouseButton.Right, rightPressedEarlier);
                        }
                    } else if (eventType == EventType.MouseUp) {
                        if (_ignoreNextLeft && e.button == 0) {
                            _ignoreNextLeft = false;
                        } else if (_ignoreNextRight && e.button == 1) {
                            _ignoreNextRight = false;
                        } else {
                            bool both;
                            if (e.button == 0) {
                                both = _mousePressed[1] != 0;
                                _ignoreNextRight = both && _mouseDragged[1] == 0;
                            } else {
                                both = _mousePressed[0] != 0;
                                _ignoreNextLeft = both && _mouseDragged[0] == 0;
                            }

                            var mouseButton = MouseButton.Left;
                            if (both)
                                mouseButton = MouseButton.Both;
                            else if (e.button == 1)
                                mouseButton = MouseButton.Right;

                            if (_mouseDragged[e.button] != 0) {
                                _mouseDragged[e.button] = 0;
                                onEndDragEvent.InvokeWhile(e, mouseButton, false, () => e.type != EventType.Used);
                            }

                            if (e.type != EventType.Used)
                                onMouseUpEvent.InvokeWhile(e, mouseButton, false, () => e.type != EventType.Used);
                        }
                    }
                }

                if (eventType == EventType.MouseDown)
                    _mousePressed[e.button] = OpenTibiaUnity.TicksMillis;
            } catch (System.Exception) {
            } finally {
                _mouseHandlerActive = false;
            }
        }

        public void OnDragEvent(Event e) {
            if (_mouseHandlerActive || e.button > _mousePressed.Length)
                return;

            try {
                bool both = (e.button == 0 ? _mousePressed[1] : _mousePressed[0]) != 0;

                MouseButton mouseButton = MouseButton.None;
                if (both)
                    mouseButton = MouseButton.Both;
                else if (e.button == 0)
                    mouseButton = MouseButton.Left;
                else if (e.button == 1)
                    mouseButton = MouseButton.Right;
                else if (e.button == 2)
                    mouseButton = MouseButton.Middle;


                if (_mouseDragged[e.button] == 0)
                    onBeginDragEvent.InvokeWhile(e, mouseButton, false, () => e.type != EventType.Used);
                else
                    onDragEvent.InvokeWhile(e, mouseButton, true, () => e.type != EventType.Used);

                _mouseDragged[e.button] = OpenTibiaUnity.TicksMillis;
            } catch (System.Exception) {
            } finally {
                _mouseHandlerActive = false;
            }
        }
        
        public bool IsModifierKeyPressed(Event e = null) {
            if (e != null)
                return e.alt || e.shift || e.control;

            return IsKeyPressed(KeyCode.LeftControl) || IsKeyPressed(KeyCode.RightControl)
                || IsKeyPressed(KeyCode.LeftShift) || IsKeyPressed(KeyCode.RightShift)
                || IsKeyPressed(KeyCode.LeftAlt) || IsKeyPressed(KeyCode.RightAlt);
        }

        public void AddKeyDownListener(Utils.EventImplPriority priority, System.Action<Event, bool> action) =>
            onKeyDownEvent.AddListener(priority, action);
        public void RemoveKeyDownListener(System.Action<Event, bool> action) =>
            onKeyDownEvent.RemoveListener(action);
        public void AddKeyUpListener(Utils.EventImplPriority priority, System.Action<Event, bool> action) =>
           onKeyUpEvent.AddListener(priority, action);
        public void RemoveKeyUpListener(System.Action<Event, bool> action) =>
            onKeyUpEvent.RemoveListener(action);

        public void AddMouseDownListener(Utils.EventImplPriority priority, System.Action<Event, MouseButton, bool> action) =>
            onMouseDownEvent.AddListener(priority, action);
        public void RemoveMouseDownListener(System.Action<Event, MouseButton, bool> action) =>
            onMouseDownEvent.RemoveListener(action);
        public void AddMouseUpListener(Utils.EventImplPriority priority, System.Action<Event, MouseButton, bool> action) =>
            onMouseUpEvent.AddListener(priority, action);
        public void RemoveMouseUpListener(System.Action<Event, MouseButton, bool> action) =>
            onMouseUpEvent.RemoveListener(action);

        public void AddDragListener(Utils.EventImplPriority priority, System.Action<Event, MouseButton, bool> action) =>
            onDragEvent.AddListener(priority, action);
        public void RemoveDragListener(System.Action<Event, MouseButton, bool> action) =>
            onDragEvent.RemoveListener(action);
        public void AddBeginDragListener(Utils.EventImplPriority priority, System.Action<Event, MouseButton, bool> action) =>
            onBeginDragEvent.AddListener(priority, action);
        public void RemoveBeginDragListener(System.Action<Event, MouseButton, bool> action) =>
            onBeginDragEvent.RemoveListener(action);
        public void AddEndDragListener(Utils.EventImplPriority priority, System.Action<Event, MouseButton, bool> action) =>
            onEndDragEvent.AddListener(priority, action);
        public void RemoveEndDragListener(System.Action<Event, MouseButton, bool> action) =>
            onEndDragEvent.RemoveListener(action);

        public EventModifiers GetRawEventModifiers() {
            EventModifiers eventModifiers = EventModifiers.None;

            if (UnityInput.GetKeyDown(KeyCode.LeftAlt) || UnityInput.GetKeyDown(KeyCode.RightAlt) || UnityInput.GetKey(KeyCode.LeftAlt) || UnityInput.GetKey(KeyCode.RightAlt))
                eventModifiers |= EventModifiers.Alt;

            if (UnityInput.GetKeyDown(KeyCode.LeftControl) || UnityInput.GetKeyDown(KeyCode.RightControl) || UnityInput.GetKey(KeyCode.LeftControl) || UnityInput.GetKey(KeyCode.RightControl))
                eventModifiers |= EventModifiers.Control;

            if (UnityInput.GetKeyDown(KeyCode.LeftShift) || UnityInput.GetKeyDown(KeyCode.RightShift) || UnityInput.GetKey(KeyCode.LeftShift) || UnityInput.GetKey(KeyCode.RightShift))
                eventModifiers |= EventModifiers.Shift;

            return eventModifiers;
        }

        public static bool IsGameObjectHighlighted(GameObject gameObject) {
            if (!gameObject || !gameObject.activeSelf)
                return false;

            var selectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (!selectedGameObject)
                return false;

            var currentTransform = selectedGameObject.transform;
            var rootTransform = currentTransform.root;
            while (currentTransform != rootTransform) {
                if (currentTransform.gameObject == gameObject)
                    return true;

                currentTransform = currentTransform.parent;
            }

            return false;
        }

        public static bool IsHighlighted(Component component) => IsGameObjectHighlighted(component?.gameObject);
    }
}
