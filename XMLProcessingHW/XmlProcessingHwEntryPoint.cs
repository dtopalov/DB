namespace XMLProcessingHW
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using System.Xml.Xsl;
    using Microsoft.Xml.XQuery;

    /// <summary>
    /// Solutions to all problems from the XML Processing Homework
    /// </summary>
    public class XmlProcessingHwEntryPoint
    {
        private const string PathToXmlFile = "../../catalogue.xml";

        /// <summary>
        /// Runs the solutions to all problems
        /// </summary>
        public static void Main()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToXmlFile);
            XmlNode root = doc.DocumentElement ?? doc.CreateElement("root");

            // Problem 2: Write program that extracts all different artists which are found in the catalog.xml.
            //  For each author you should print the number of albums in the catalogue.
            //  Use the DOM parser and a hash - table.
            PrintNumbersOfAlbumsForEachArtist(root);

            // Problem 3: Implement the previous using XPath.
            PrintArtistsNumberOfAlbumsUsingXPath(root);

            // Problem 4: Using the DOM parser write a program to delete from catalog.xml all albums having price > 20.
            DeleteAlbumsByPrice(root, 20.0);

            // Check that albums are deleted:
            PrintNumbersOfAlbumsForEachArtist(root);

            // Problem 5: Write a program, which using XmlReader extracts all song titles from catalog.xml.
            var songTitles = ExtractSongTitlesFromCatalogue(PathToXmlFile);
            Console.WriteLine("Song titles: " + string.Join(", ", (songTitles as List<string>).ToArray()));

            // Problem 6: Rewrite the same using XDocument and LINQ query.
            XDocument xDoc = XDocument.Load(PathToXmlFile);
            var songTitlesUsingLinq = from songs in xDoc.Descendants("title") select songs.Value.Trim();
            Console.WriteLine("Song titles (using LINQ): " + string.Join(", ", songTitlesUsingLinq));

            // Problem 7: In a text file we are given the name, address and phone number of given person (each at a single line).
            // Write a program, which creates new XML document, which contains these data in structured XML format.
            CreateXmlPhonebook("../../phonebook.txt");

            // Problem 8: Write a program, which (using XmlReader and XmlWriter) reads the file catalog.xml and creates the file album.xml,
            // in which stores in appropriate way the names of all albums and their authors.
            CreateAlbumsXml(PathToXmlFile);

            // Problem 9: Write a program to traverse given directory and write to a XML file its contents together with all subdirectories and files.
            // Use tags < file > and < dir > with appropriate attributes.
            // For the generation of the XML document use the class XmlWriter.
            using (var writer = new XmlTextWriter("../../traverseWithXmlWriter.xml", Encoding.UTF8))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("DirectoriesRoot");
                CreateFileSystemXmlTreeUsingXmlWriter("../../..", writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }

            // Problem 10: Rewrite the last exercises using XDocument, XElement and XAttribute.
            var xDocument = new XDocument();
            xDocument.Add(CreateFileSystemXmlTree("../../../"));
            xDocument.Save("../../traverseWithXElement.xml");

            // Problem 11: Write a program, which extract from the file catalog.xml the prices for all albums, published 5 years ago or earlier.
            // Use XPath query.
            doc.Load(PathToXmlFile);
            root = doc.DocumentElement;

            // returns all (no albums in the catalogue are newer...)
            var oldAlbumsPrices = root.SelectNodes("album/price[../year/text() < 2010]");

            //// var oldAlbumsPrices = root.SelectNodes("album/price[../year/text() < 1980]"); // returns 2 albums' prices
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("Prices of the albums, published before 2010: ");
            foreach (var price in oldAlbumsPrices)
            {
                Console.WriteLine((price as XmlElement).InnerXml.Trim());
            }

            // Problem 12: Rewrite the previous using LINQ query.
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("Prices of the albums, published before 2010 (using LINQ): ");
            var oldAlbumsPricesUsingLinq = from album in xDoc.Descendants("album")
                              where int.Parse(album.Element("year").Value) < 2010
                              select album.Descendants("price").FirstOrDefault();
            foreach (var price in oldAlbumsPricesUsingLinq)
            {
                Console.WriteLine(price.Value.Trim());
            }

            // Problem 13: Create an XSL stylesheet, which transforms the file catalog.xml into HTML document,
            // formatted for viewing in a standard Web-browser.
            // Problem 14: Write a C# program to apply the XSLT stylesheet transformation on the file catalog.xml
            // using the class XslTransform.
            XslCompiledTransform catalogueXslt = new XslCompiledTransform();
            catalogueXslt.Load("../../catalogue.xslt");
            catalogueXslt.Transform(PathToXmlFile, "../../catalogue.html");

            // Problem 15:
            // *Read some tutorial about the XQuery language.
            // Implement the XML to HTML transformation with XQuery (instead of XSLT).
            // Download some open source XQuery library for .NET and execute the XQuery to transform the catalog.xml to HTML.
            XQueryNavigatorCollection col = new XQueryNavigatorCollection();

            // Add the XML document catalogue.xml to the collection using cat as the name to reference.
            col.AddNavigator("../../catalogue.xml", "cat");
            var expr = new XQueryExpression(
                "<html><body><head><title>Catalogue</title></head>" +
                "<h1>Catalogue generated using XQuery</h1>" +
                "{For $a IN document(\"cat\")/catalogue/album " +
                "RETURN <div><strong>Title:</strong> {$a/name/text()}<br />" +
                "<strong>Artist:</strong> {$a/artist/text()}<br />" +
                "<strong>Year:</strong> {$a/year/text()}<br />" +
                "<strong>Producer:</strong> {$a/producer/text()}<br />" +
                "<strong>Price:</strong> {$a/price/text()}<br />" +
                "<strong>Songs:</strong><ol>{For $s IN $a/songs/song RETURN <li>{$s/title/text()}</li>}</ol>" +
                "</div><hr />}</body></html>");
            StreamWriter str = new StreamWriter("../../catalogueUsingXQuery.html");
            XQueryNavigator nav = expr.Execute(col);
            nav.ToXml(str);
            str.Close();

            // Problem 16:
            // Using Visual Studio generate an XSD schema for the file catalog.xml.
            // Write a C# program that takes an XML file and an XSD file (schema) and validates the XML file against the schema.
            // Test it with valid XML catalogs and invalid XML catalogs.
            string xsdMarkup = File.ReadAllText("../../catalogue.xsd");
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(string.Empty, XmlReader.Create(new StringReader(xsdMarkup)));
            XDocument valid = XDocument.Load(PathToXmlFile);
            XDocument invalid = new XDocument(
                new XElement(
                    "Root",
                    new XElement("Child1", "content1"),
                    new XElement("Child2", "content2")));
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("Validating valid document:");
            bool errors = false;
            valid.Validate(schemas, (o, e) =>
            {
                Console.WriteLine("{0}", e.Message);
                errors = true;
            });
            Console.WriteLine("Valid {0}", errors ? "did not validate" : "validated");
            Console.WriteLine();
            Console.WriteLine("Validating invalid document:");
            errors = false;
            invalid.Validate(schemas, (o, e) =>
            {
                Console.WriteLine("{0}", e.Message);
                errors = true;
            });
            Console.WriteLine("doc2 {0}", errors ? "did not validate" : "validated");
        }

        private static void PrintNumbersOfAlbumsForEachArtist(XmlNode root)
        {
            var artists = new Hashtable();

            foreach (XmlElement album in root.ChildNodes)
            {
                if (artists.ContainsKey(album["artist"].InnerText))
                {
                    (artists[album["artist"].InnerText] as List<string>).Add(album["name"].InnerText);
                }
                else
                {
                    artists.Add(album["artist"].InnerText, new List<string> { album["name"].InnerText });
                }
            }

            foreach (var key in artists.Keys)
            {
                Console.WriteLine($"{key} Number of albums: {(artists[key] as List<string>).Count}");
            }

            Console.WriteLine(new string('-', 50));
        }

        private static void PrintArtistsNumberOfAlbumsUsingXPath(XmlNode root)
        {
            var artistsAndNumberOfAlbums = new Hashtable();
            var albums = root.SelectNodes("album");

            foreach (XmlElement album in albums)
            {
                if (artistsAndNumberOfAlbums.ContainsKey(album["artist"].InnerText))
                {
                    (artistsAndNumberOfAlbums[album["artist"].InnerText] as List<string>).Add(album["name"].InnerText);
                }
                else
                {
                    artistsAndNumberOfAlbums.Add(album["artist"].InnerText, new List<string> { album["name"].InnerText });
                }
            }

            foreach (var key in artistsAndNumberOfAlbums.Keys)
            {
                Console.WriteLine($"{key} Number of albums: {(artistsAndNumberOfAlbums[key] as List<string>).Count}");
            }

            Console.WriteLine(new string('-', 50));
        }

        private static void DeleteAlbumsByPrice(XmlNode root, double minPrice)
        {
            bool deletePrevious = false;

            foreach (XmlElement album in root.ChildNodes)
            {
                if (deletePrevious)
                {
                    root.RemoveChild(album.PreviousSibling);
                    deletePrevious = false;
                }

                if (double.Parse(album["price"].InnerText) > minPrice)
                {
                    Console.WriteLine($"{album["name"].InnerText} deleted!");
                    deletePrevious = true;
                }
            }

            if (deletePrevious)
            {
                root.RemoveChild(root.LastChild);
            }
        }

        private static IList ExtractSongTitlesFromCatalogue(string pathToCatalogue)
        {
            var songTitles = new List<string>();

            using (XmlReader reader = XmlReader.Create(pathToCatalogue))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "title")
                    {
                        songTitles.Add(reader.ReadElementString().Trim());
                    }
                }
            }

            return songTitles;
        }

        private static void CreateXmlPhonebook(string pathToTxtFile)
        {
            int lineNumber = 0;
            var writer = new XmlTextWriter("../../phonebook.xml", Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement("entries");
            using (var reader = new StreamReader(pathToTxtFile))
            {
                while (!reader.EndOfStream)
                {
                    switch (lineNumber % 3)
                    {
                        case 0: writer.WriteStartElement("entry");
                                writer.WriteElementString("name", reader.ReadLine());
                            break;
                        case 1: writer.WriteElementString("address", reader.ReadLine());
                            break;
                        case 2: writer.WriteElementString("phone", reader.ReadLine());
                                writer.WriteEndElement();
                            break;
                    }

                    lineNumber++;
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            writer.Dispose();
        }

        private static void CreateAlbumsXml(string pathToXmlFile)
        {
            var writer = new XmlTextWriter("../../albums.xml", Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement("albums");

            using (var reader = XmlReader.Create(pathToXmlFile))
            {
                while (reader.Read())
                {
                    switch (reader.Name)
                    {
                        case "album":
                            if (reader.IsStartElement())
                            {
                                writer.WriteStartElement("album");
                            }

                            break;
                        case "name":
                            writer.WriteElementString("title", reader.ReadElementContentAsString());
                            break;
                        case "artist":
                            writer.WriteElementString("artist", reader.ReadElementContentAsString());
                            writer.WriteEndElement();
                            break;
                    }
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private static XElement CreateFileSystemXmlTree(string source)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(source);
            var result = new XElement(
                "Dir",
                new XAttribute("Name", directoryInfo.Name),
                Directory.GetDirectories(source).Select(dir => CreateFileSystemXmlTree(dir)),
                directoryInfo.GetFiles().Select(fileName => new XElement("File", new XAttribute("Name", fileName.Name), new XAttribute("Size", fileName.Length))));

            return result;
        }

        private static void CreateFileSystemXmlTreeUsingXmlWriter(string source, XmlWriter writer)
        {
            var directoryInfo = new DirectoryInfo(source);
            var folders = directoryInfo.GetDirectories();

            foreach (var folder in folders)
            {
                writer.WriteStartElement("Dir");
                writer.WriteAttributeString("Name", folder.Name);
                CreateFileSystemXmlTreeUsingXmlWriter(folder.FullName, writer);
                writer.WriteEndElement();
            }

            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                writer.WriteStartElement("File");
                writer.WriteAttributeString("Name", file.Name);
                writer.WriteAttributeString("Size", file.Length.ToString());
                writer.WriteEndElement();
            }
        }
    }
}