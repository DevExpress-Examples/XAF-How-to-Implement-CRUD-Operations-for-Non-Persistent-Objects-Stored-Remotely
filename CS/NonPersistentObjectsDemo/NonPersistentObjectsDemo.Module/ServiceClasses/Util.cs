using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonPersistentObjectsDemo.Module {

    public class GenHelper {
        private static Random srnd;
        private static List<string> words;
        private static List<string> fnames;
        private static List<string> lnames;
        static GenHelper() {
            srnd = new Random();
            words = CreateWords(12000);
            fnames = CreateNames(200);
            lnames = CreateNames(500);
        }
        private static List<string> CreateWords(int number) {
            var items = new HashSet<string>();
            while(number > 0) {
                if(items.Add(CreateWord())) {
                    number--;
                }
            }
            return items.ToList();
        }
        private static string MakeTosh(Random rnd, int length) {
            var chars = new char[length];
            for(int i = 0; i < length; i++) {
                chars[i] = (char)('a' + rnd.Next(26));
            }
            return new String(chars);
        }
        private static string CreateWord() {
            return MakeTosh(srnd, 1 + srnd.Next(13));
        }
        private static List<string> CreateNames(int number) {
            var items = new HashSet<string>();
            while(number > 0) {
                if(items.Add(ToTitle(CreateWord()))) {
                    number--;
                }
            }
            return items.ToList();
        }
        public static string ToTitle(string s) {
            if(string.IsNullOrEmpty(s))
                return s;
            return string.Concat(s.Substring(0, 1).ToUpper(), s.Substring(1));
        }

        private Random rnd;
        public GenHelper() {
            rnd = new Random();
        }
        public GenHelper(int seed) {
            rnd = new Random(seed);
        }
        public int Next(int max) {
            return rnd.Next(max);
        }
        public string MakeTosh(int length) {
            return MakeTosh(rnd, length);
        }
        public string MakeBlah(int length) {
            var sb = new StringBuilder();
            for(var i = 0; i <= length; i++) {
                if(sb.Length > 0) {
                    sb.Append(" ");
                }
                sb.Append(GetWord());
            }
            return sb.ToString();
        }
        public string MakeBlahBlahBlah(int length, int plength) {
            var sb = new StringBuilder();
            for(var i = 0; i <= length; i++) {
                if(sb.Length > 0) {
                    sb.Append(" ");
                }
                var w = ToTitle(MakeBlah(3 + rnd.Next(plength))) + ".";
                sb.Append(w);
            }
            return sb.ToString();
        }
        public string GetFullName() {
            return string.Concat(GetFName(), " ", GetLName());
        }
        private string GetFName() {
            return fnames[rnd.Next(fnames.Count)];
        }
        private string GetLName() {
            return lnames[rnd.Next(lnames.Count)];
        }
        private string GetWord() {
            return words[rnd.Next(words.Count)];
        }
    }
}
