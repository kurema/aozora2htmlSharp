using System.Text.RegularExpressions;

using var sr = new StreamReader(args[0], System.Text.Encoding.UTF8);
using var sw = new StreamWriter(args[1], false, System.Text.Encoding.UTF8);;
var text = await sr.ReadToEndAsync();
var regex = new Regex(@"[\n\r]*[\s\t]*private static Regex\? _(\w+) = null;[\n\r]*[\s\t]*public static Regex \w+\s*=>\s*_\w+ \?\?= new Regex\(@""([^\""]+)"", RegexOptions.Compiled\);", RegexOptions.Singleline);
var result = regex.Replace(text, """"

        private static Regex? _$1 = null;
#if NET7_0_OR_GREATER
        [GeneratedRegex(@"$2")]
        private static partial Regex _$1_GEN();
        public static Regex $1 => _$1 ??= _$1_GEN();
#else
        public static Regex $1 => _$1 ??= new Regex(@"$2", RegexOptions.Compiled);
#endif

""""
);
await sw.WriteLineAsync(result);