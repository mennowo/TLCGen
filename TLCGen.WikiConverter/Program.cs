using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace TLCGen.WikiConverter
{
    /// <summary>
    /// This program converts Wordpress XML data to html. The code is oriented towards
    /// converting "knowledge base" articles, so they can be published as a whole.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            using (var s = new StreamReader(@"C:\Users\menno\Documents\temp\codingconnected.wordpress.2018-09-11.xml", Encoding.GetEncoding(65001), true))
            {
                doc.Load(s);
            }

            var mainNode = doc.DocumentElement.FirstChild;

            Dictionary<string, List<string>> Categories = new Dictionary<string, List<string>>();
            
            foreach (XmlNode node in mainNode.ChildNodes)
            {
                if(node.Name == "item")
                {
                    Console.WriteLine("item found");
                    var category = "";
                    var title = "";
                    var text = "";
                    foreach (XmlNode itemNode in node.ChildNodes)
                    {
                        if (itemNode.Name == "category")
                        {
                            var rawText = itemNode.InnerText;
                            category = Regex.Replace(rawText, @"\[\:([a-z]+)?\]", "");
                            category = Regex.Replace(category, @"\<\!\[CDATA\[\[?:?([a-z]+)?\]?", "");
                            category = Regex.Replace(category, @"\[?:?\]?\]\]\>", "");
                        }
                        if (itemNode.Name == "title")
                        {
                            Console.WriteLine(itemNode.InnerText);
                            title = $"<h2>{itemNode.InnerText}</h2>";
                        }
                        if (itemNode.Name == "content:encoded")
                        {
                            var rawText = itemNode.InnerText;
                            text = Regex.Replace(rawText, @"\[\:([a-z]+)?\]", "");
                            text = Regex.Replace(text, @"\<\!\[CDATA\[\[?:?([a-z]+)?\]?", "");
                            text = Regex.Replace(text, @"\[?:?\]?\]\]\>", "");
                            text = Regex.Replace(text, @"\<h4\>", "<h6>");
                            text = Regex.Replace(text, @"\<h2\>", "<h5>");
                            text = Regex.Replace(text, @"\<h3\>", "<h4>");
                            text = Regex.Replace(text, @"\<h1\>", "<h3>");
                            text = Regex.Replace(text, @"\</h4\>", "</h6>");
                            text = Regex.Replace(text, @"\</h2\>", "</h5>");
                            text = Regex.Replace(text, @"\</h3\>", "</h3>");
                            text = Regex.Replace(text, @"\</h1\>", "</h2>");
                        }
                    }
                    if (title != "")
                    {
                        if (!Categories.ContainsKey(category))
                        {
                            Categories.Add(category, new List<string>());
                        }
                        Categories[category].Add(title);
                        Categories[category].Add(text);
                    }
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body>");
            sb.Append(
@"<style>
body {
    font: arial;
}

html *
{
   font-family: Arial !important;
}

h1 {
    color: maroon;
}
</style> ");
            foreach (var cat in Categories)
            {
                sb.AppendLine($"<h1>{cat.Key}</h1>");
                foreach(var item in cat.Value)
                {
                    sb.Append(item);
                }
            }

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText(@"C:\Users\menno\Documents\temp\codingconnected.wordpress.2018-09-11.html", sb.ToString(), Encoding.UTF8);

            Console.WriteLine("press enter now");
        }
    }
}
