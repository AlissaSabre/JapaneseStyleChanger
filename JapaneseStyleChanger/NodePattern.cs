using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JapaneseStyleChanger
{
    public class NodePattern
    {
        private class Rule
        {
            private class Finder
            {
                public readonly Func<WNode, bool> Matcher;

                public readonly int Offset;

                public Finder(int offset, Func<WNode, bool> matcher)
                {
                    Offset = offset;
                    Matcher = matcher;
                }
            }

            private class Replacer
            {

            }

            private readonly List<Finder> Finders = new List<Finder>();

            private readonly List<Replacer> Replacers = new List<Replacer>();

            public int MaxOffset { get; set; }

            public void AddFinder(int offset, Func<WNode, bool> matcher)
            {
                Finders.Add(new Finder(offset, matcher));
            }

            public bool TryRewrite(EditBuffer buffer, int index)
            {
                if (index + MaxOffset >= buffer.Count) return false;
                if (!Finders.All(f => f.Matcher(buffer[index + f.Offset]))) return false;

                


                return true;
            }
        }

        private enum Pattern { Single, Except, OneOf }

        public void ReadRules(Stream stream)
        {
            foreach (var xrule in XElement.Load(stream).Elements("Rule"))
            {
                var rule = new Rule();

                var find_nodes = xrule.Element("Find").Elements("Node").ToList();
                rule.MaxOffset = find_nodes.Count - 1;
                for (int offset = 0; offset < find_nodes.Count; offset++)
                {
                    foreach (var attr in find_nodes[offset].Attributes())
                    {
                        Pattern pattern;
                        string[] values = null;
                        var value = (string)attr;
                        if (value.StartsWith("!"))
                        {
                            pattern = Pattern.Except;
                            value = value.Substring(1);
                        }
                        else if (value.Contains("|"))
                        {
                            pattern = Pattern.OneOf;
                            values = value.Split('|');
                        }
                        else
                        {
                            pattern = Pattern.Single;
                        }

                        switch (attr.Name.LocalName)
                        {
                            case "Pos1":
                                rule.AddFinder(offset, BuildMatcher(pattern, node => node.Pos1, value, values));
                                break;
                            case "Pos2":
                                rule.AddFinder(offset, BuildMatcher(pattern, node => node.Pos2, value, values));
                                break;
                            case "CForm":
                                rule.AddFinder(offset, BuildMatcher(pattern, node => node.CForm, value, values));
                                break;
                            case "Lemma_id":
                                int.TryParse(value, out var int_value);
                                var int_values = values?.Select(s => int.Parse(s)).ToArray();
                                rule.AddFinder(offset, BuildMatcher(pattern, node => node.Lemma_id, int_value, int_values));
                                break;
                            default:
                                throw new ApplicationException();
                        }
                    }
                }
            }


            throw new NotImplementedException();
        }

        private static Func<WNode, bool> BuildMatcher<T>(Pattern pattern, Func<WNode, T> getter, T value, T[] values) where T: IEquatable<T>
        {
            switch (pattern)
            {
                case Pattern.Single: return node => value.Equals(getter(node));
                case Pattern.Except: return node => !value.Equals(getter(node));
                case Pattern.OneOf: return node => Array.IndexOf(values, getter(node)) >= 0;
                default: throw new ApplicationException();
            }
        }

        public bool FindAndReplace(EditBuffer buffer, int index)
        {
            throw new NotImplementedException();
        }
    }
}
