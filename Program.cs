using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Lab4
{
    public class Program
    {
        public static void Main()
        {
            // Test 1: Skracanie tekstów dowolnej długości
            TestVariableLength();

            // Test 2: Jednobitowa zmiana -> duży wpływ
            TestBitFlip();

            // Test 3: Odległość Hamminga między hashami różnych tekstów
            TestHammingDistance();

            // Test 4: Stała długość skrótu
            TestFixedHashLength();

            // Test 5: Porównanie czasu SHA256 własnego vs. bibliotecznego
            TestHashTiming();
        }

        public static byte[] ComputeSha256Custom(string input) => Sha256Custom.ComputeHash(input);

        public static byte[] ComputeSha256DotNet(string input)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        public static void TestVariableLength()
        {
            Console.WriteLine("1. Czy można skrócić tekst dowolnej długości?");
            string[] texts = new[]
            {
                "a",
                "abc",
                new string('x', 1_000),
                new string('y', 1_000_000)
            };

            foreach (string text in texts)
            {
                byte[] hash = ComputeSha256Custom(text);
                Console.WriteLine($"Tekst długości {text.Length} -> Hash: {hash.ToHexString()}");
            }
        }

        public static void TestBitFlip()
        {
            Console.WriteLine("\n2. Czy zmiana jednego bitu istotnie zmienia hash?");
            string original = "Test message";
            string flipped = "test message";

            byte[] hash1 = ComputeSha256Custom(original);
            byte[] hash2 = ComputeSha256Custom(flipped);

            Console.WriteLine($"Oryginał: {original}");
            Console.WriteLine($"Zmodyfikowany: {flipped}");
            Console.WriteLine($"Hash 1: {hash1.ToHexString()}");
            Console.WriteLine($"Hash 2: {hash2.ToHexString()}");
            Console.WriteLine($"Odległość Hamminga: {HammingDistance(hash1, hash2)} bitów");
        }

        public static void TestHammingDistance()
        {
            Console.WriteLine("\n3. Jaka jest odległość Hamminga dla różnych tekstów?");
            byte[] hash1 = ComputeSha256Custom("kot");
            byte[] hash2 = ComputeSha256Custom("pies");

            Console.WriteLine($"Hash(kot):  {hash1.ToHexString()}");
            Console.WriteLine($"Hash(pies): {hash2.ToHexString()}");
            Console.WriteLine($"Odległość Hamminga: {HammingDistance(hash1, hash2)} bitów");
        }

        public static void TestFixedHashLength()
        {
            Console.WriteLine("\n4. Czy hash zawsze ma tę samą długość?");
            string[] texts = new[]
            {
                "a",
                "abc",
                new string('x', 1_000),
                new string('y', 1_000_000)
            };

            foreach (string text in texts)
            {
                byte[] hash = ComputeSha256Custom(text);
                Console.WriteLine($"Tekst długości {text.Length} -> Długość hash: {hash.Length * 8}");
            }
        }

        public static void TestHashTiming()
        {
            Console.WriteLine("\n5. Czas tworzenia hashy: własny vs SHA256 z .NET");

            string input = new string('x', 1_000_000);
            byte[] data = Encoding.UTF8.GetBytes(input);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
                ComputeSha256Custom(input);
            sw.Stop();
            Console.WriteLine($"Własna implementacja SHA256: {sw.ElapsedMilliseconds} ms (10 hashy)");

            sw.Restart();
            for (int i = 0; i < 10; i++)
                ComputeSha256DotNet(input);
            sw.Stop();
            Console.WriteLine($"SHA256 z .NET: {sw.ElapsedMilliseconds} ms (10 hashy)");
        }

        // Oblicza odległość Hamminga między dwoma hashami (czyli ile bitów się różni). 
        private static int HammingDistance(byte[] a, byte[] b)
        {
            int distance = 0;
            for (int i = 0; i < a.Length; i++)
            {
                byte xor = (byte)(a[i] ^ b[i]);
                distance += CountBits(xor);
            }
            return distance;
        }

        // Liczy ile bitów jest ustawionych na 1 w bajcie
        private static int CountBits(byte b)
        {
            int count = 0;
            while (b != 0)
            {
                count += b & 1;
                b >>= 1;
            }
            return count;
        }
    }
}