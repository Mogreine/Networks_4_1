using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace TaskServer.src
{
    class CongruentialGenerator
    {
        private ulong _curr_element;
        private const long a = 48271;
        private const long c = 0;
        private uint m = 1u << 31;

        public CongruentialGenerator(uint seed)
        {
            m--;
            _curr_element = seed;
        }

        public CongruentialGenerator(BigInteger seed)
        {
            m--;
            _curr_element = Convert.ToUInt64((seed % m).ToString());
        }

        public ulong Rand()
        {
            _curr_element = (a * _curr_element + c) % m;
            return _curr_element;
        }

        public string RandSequence(int size)
        {
            var res = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                res.Append(Rand() & 1);
            }
            return res.ToString();
        }

        public List<byte> RandByteSequence(int size)
        {
            var res = new List<byte>(size);
            for (int i = 0; i < size; i++)
            {
                byte bt = 0;
                for (int j = 0; j < 8; j++)
                {
                    bt += Convert.ToByte((Rand() & 1) << j);
                }
                res.Add(bt);
            }

            return res;
        }
    }
}
