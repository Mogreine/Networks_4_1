using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TaskServer.src
{
    class Cypher
    {
        public static byte[] Encrypt(byte[] msg, string keyString)
        {
            // Получаем ключ для шифрования
            var keyHash = MD4Hash.Hash(keyString);
            // var message_hash = MD4Hash.Hash(LeftTextField.Text);

            // Генерируем случайную последовательность битов
            var rnd = new CongruentialGenerator(BigInteger.Parse("0" + keyHash, NumberStyles.AllowHexSpecifier));
            var rand_seq = rnd.RandByteSequence(msg.Length);

            var message_xor = new byte[msg.Length];
            for (int i = 0; i < msg.Length; i++)
            {
                message_xor[i] = (byte)(msg[i] ^ rand_seq[i]);
            }

            return message_xor;
        }

        public static byte[] Decrypt(byte[] msg, string keyString)
        {
            // Получаем ключ для шифрования
            var message_hash = MD4Hash.Hash(keyString);
            
            // Генерируем случайную последовательность битов
            var rnd = new CongruentialGenerator(BigInteger.Parse("0" + message_hash, NumberStyles.AllowHexSpecifier));
            var rand_seq = rnd.RandByteSequence(msg.Length);

            var message_xor = new byte[msg.Length];
            for (int i = 0; i < msg.Length; i++)
            {
                message_xor[i] = (byte)(msg[i] ^ rand_seq[i]);
            }

            return message_xor;
        }
    }
}
