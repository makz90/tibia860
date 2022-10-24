﻿using System;
using System.Text;

namespace OpenTibia.IO
{
    public class ByteArrayStreamWriter
    {
        public ByteArrayStreamWriter(ByteArrayStream stream)
        {
            this.stream = stream;
        }

        private ByteArrayStream stream;

        public ByteArrayStream BaseStream
        {
            get
            {
                return stream;
            }
        }

        public void Write(byte value)
        {
            stream.WriteByte(value);
        }
        
        public void Write(bool value)
        {
            Write( BitConverter.GetBytes(value) );
        }

        public void Write(short value)
        {
            Write( BitConverter.GetBytes(value) );
        }

        public void Write(ushort value)
        {
            Write( BitConverter.GetBytes(value) );
        }

        public void Write(int value)
        {
            Write( BitConverter.GetBytes(value) );
        }

        public void Write(uint value)
        {
            Write( BitConverter.GetBytes(value) );
        }

        public void Write(long value)
        {
            Write( BitConverter.GetBytes(value) );
        }

        public void Write(ulong value)
        {
            Write( BitConverter.GetBytes(value) );
        }

        public void Write(string value)
        {
            if (value == null)
            {
                Write( (ushort)0 );
            }
            else
            {
                Write( (ushort)value.Length );

                Write( Encoding.Default.GetBytes(value) );
            }
        }

        public void Write(byte[] buffer)
        {
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}