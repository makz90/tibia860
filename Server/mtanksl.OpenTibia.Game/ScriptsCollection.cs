﻿using OpenTibia.Game.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenTibia.Game
{
    public class ScriptsCollection
    {
        private Server server;

        public ScriptsCollection(Server server)
        {
            this.server = server;
        }

        private List<IScript> scripts = new List<IScript>();

        public void Start()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IScript).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract ) )
            {
                IScript script = (IScript)Activator.CreateInstance(type);

                scripts.Add(script);
            }

            foreach (var script in scripts)
            {
                script.Start(server);
            }
        }

        public T GetScript<T>()
        {
            return scripts.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetScripts<T>()
        {
            return scripts.OfType<T>();
        }

        public void Stop()
        {
            foreach (var script in scripts)
            {
                script.Stop(server);
            }
        }
    }
}