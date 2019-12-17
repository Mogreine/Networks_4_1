using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskServer.src
{
    static class MD4Hash
    {
        public static string Hash(string input)
        {
            // Переводим сообщение в байты
            List<byte> bytes = Encoding.ASCII.GetBytes(input).ToList();
            uint bitCount = (uint)(bytes.Count) * 8;
            bytes.Add(128);

            // Пока size(message) % 512 != 448 (в битах)
            while (bytes.Count % 64 != 56)
                bytes.Add(0);

            // Сжимаем сообщение в блоки по 32 бита, ибо потом оно будет обрабатываться блоками по 16 32-битных чисел
            var word_chunks = new List<uint>();
            for (int i = 0; i + 3 < bytes.Count; i += 4)
                word_chunks.Add(bytes[i] | (uint)bytes[i + 1] << 8 | (uint)bytes[i + 2] << 16 | (uint)bytes[i + 3] << 24);
            word_chunks.Add(bitCount);

            // Добавляем еще 1 слово, т.к. длина сообщения должна быть представлена в 64-битном формате
            word_chunks.Add(0);

            // Инициализируем хеш и начинаем раунды
            uint a = 0x67452301, b = 0xefcdab89, c = 0x98badcfe, d = 0x10325476;
            Func<uint, uint, uint> rol = (x, y) => x << (int)y | x >> 32 - (int)y;
            for (int q = 0; q + 15 < word_chunks.Count; q += 16)
            {
                var chunk = word_chunks.GetRange(q, 16);
                uint aa = a, bb = b, cc = c, dd = d;
                Action<Func<uint, uint, uint, uint>, uint[]> round = (f, y) =>
                {
                    foreach (uint i in new[] { y[0], y[1], y[2], y[3] })
                    {
                        a = rol(a + f(b, c, d) + chunk[(int)(i + y[4])] + y[12], y[8]);
                        d = rol(d + f(a, b, c) + chunk[(int)(i + y[5])] + y[12], y[9]);
                        c = rol(c + f(d, a, b) + chunk[(int)(i + y[6])] + y[12], y[10]);
                        b = rol(b + f(c, d, a) + chunk[(int)(i + y[7])] + y[12], y[11]);
                    }
                };
                round((x, y, z) => (x & y) | (~x & z), new uint[] { 0, 4, 8, 12, 0, 1, 2, 3, 3, 7, 11, 19, 0 });
                round((x, y, z) => (x & y) | (x & z) | (y & z), new uint[] { 0, 1, 2, 3, 0, 4, 8, 12, 3, 5, 9, 13, 0x5a827999 });
                round((x, y, z) => x ^ y ^ z, new uint[] { 0, 2, 1, 3, 0, 8, 4, 12, 3, 9, 11, 15, 0x6ed9eba1 });
                a += aa; b += bb; c += cc; d += dd;
            }

            // Объединяем хеш константы в 1 массив в виде байтов
            byte[] outBytes = new[] { a, b, c, d }.SelectMany(BitConverter.GetBytes).ToArray();
            // Переводим байты в 16-ричное число
            return BitConverter.ToString(outBytes).Replace("-", "").ToLower();
        }
    }
}
