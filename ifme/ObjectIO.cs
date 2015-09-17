using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ifme
{
	class ObjectIO
	{
		/* 
		Source: http://stackoverflow.com/a/22416929

		Example:
		// Write the list of salesman objects to file.
		WriteToXmlFile<List<salesman>>("C:\salesmen.txt", salesmanList);

		// Read the list of salesman objects from the file back into a variable.
		List<salesman> salesmanList = ReadFromXmlFile<List<salesman>>("C:\salesmen.txt");
		*/

		public static string FileName;

		/// <summary>
		/// Writes the given object instance to a binary file.
		/// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
		/// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
		/// </summary>
		/// <typeparam name="T">The type of object being written to the XML file.</typeparam>
		/// <param name="filePath">The file path to write the object instance to.</param>
		/// <param name="objectToWrite">The object instance to write to the XML file.</param>
		/// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
		public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
		{
			using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
			{
				var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				binaryFormatter.Serialize(stream, objectToWrite);
			}
		}

		/// <summary>
		/// Reads an object instance from a binary file.
		/// </summary>
		/// <typeparam name="T">The type of object to read from the XML.</typeparam>
		/// <param name="filePath">The file path to read the object instance from.</param>
		/// <returns>Returns a new instance of the object read from the binary file.</returns>
		public static T ReadFromBinaryFile<T>(string filePath)
		{
			using (Stream stream = File.Open(filePath, FileMode.Open))
			{
				var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				return (T)binaryFormatter.Deserialize(stream);
			}
		}

		/// <summary>
		/// Writes the given object instance to an XML file.
		/// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
		/// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
		/// <para>Object type must have a parameterless constructor.</para>
		/// </summary>
		/// <typeparam name="T">The type of object being written to the file.</typeparam>
		/// <param name="filePath">The file path to write the object instance to.</param>
		/// <param name="objectToWrite">The object instance to write to the file.</param>
		/// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
		public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
		{
			TextWriter writer = null;
			try
			{
				var serializer = new XmlSerializer(typeof(T));
				writer = new StreamWriter(filePath, append);
				serializer.Serialize(writer, objectToWrite);
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}

		/// <summary>
		/// Reads an object instance from an XML file.
		/// <para>Object type must have a parameterless constructor.</para>
		/// </summary>
		/// <typeparam name="T">The type of object to read from the file.</typeparam>
		/// <param name="filePath">The file path to read the object instance from.</param>
		/// <returns>Returns a new instance of the object read from the XML file.</returns>
		public static T ReadFromXmlFile<T>(string filePath) where T : new()
		{
			TextReader reader = null;
			try
			{
				var serializer = new XmlSerializer(typeof(T));
				reader = new StreamReader(filePath);
				return (T)serializer.Deserialize(reader);
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
		}

		/// <summary>
		/// To validate current file is valid XML format or not
		/// </summary>
		/// <param name="xmlFile">Full path of XML file</param>
		/// <returns>Return true if valid, false otherwise</returns>
		public static bool IsValidXml(string file)
		{
			try
			{
				XDocument.Load(file);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
