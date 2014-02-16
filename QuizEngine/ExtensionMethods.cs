using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;


namespace QuizEngine
{
    public static class ThreadSafeRandom
    {
        [DllImport("Kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [ThreadStatic]
        private static Random _local;

        public static Random ThisThreadsRandom
        {
            get { return _local ?? (_local = new Random(unchecked(Environment.TickCount * 31 + (int)GetCurrentThreadId()))); }
        }
    }

    static class ExtensionMethods
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static async Task<string> ReadQuizFromFile(string fileName)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            var file = await folder.GetFileAsync(fileName);
            var quizText = await Windows.Storage.FileIO.ReadTextAsync(file);
            return quizText;
        }

        public static string Serialize(object instance)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(instance.GetType());
                serializer.WriteObject(stream, instance);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                { return reader.ReadToEnd(); }
            }
        }

        public static T Deserialize<T>(string json)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }
    }


}
