using Newtonsoft.Json;

namespace Serializering
{
	internal class Program
	{

		static void Main(string[] args)
		{
			Test test = new Test
			{
				Color = "blue",
				Size = "large"
			};

			string jsonString = JsonConvert.SerializeObject(test);
			Console.WriteLine(jsonString);
			Console.WriteLine(test.Size + ", " + test.Color);

			Test deserealizedTest = JsonConvert.DeserializeObject<Test>(jsonString)!;
			Console.WriteLine(deserealizedTest.Size + deserealizedTest.Color);
		}
	}

	class Test
	{
		public string Color { get; set; } = null!;
		public string Size { get; set; } = null!;
	}
}
