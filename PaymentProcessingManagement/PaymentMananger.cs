using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using babysittingIL.Constants;

namespace babysittingIL.Payments
{
	class PaymentsManager
	{
		public static async Task RunTestTransaction()
		{
   		    // Replace with your own values
			string baseUrl = "https://sandbox.api.visa.com";
			string apiKey = Consts.VisaAPIKey;
			string sharedSecret = Consts.VisaAPISharedSecret;
			string resourcePath = "/visadirect/fundstransfer/v1/pullfundstransactions";
   		    // Build the request body
			var request = new
			{
				acquirerCountryCode = "840",
				acquiringBin = "408999",
				amount = "124.05",
				surcharge = "15",
				businessApplicationId = "AA",
				cardAcceptor = new
				{
					address = new
					{
						country = "USA",
						county = "081",
						state = "CA",
						zipCode = "94404"
					},
					idCode = "ABCD1234ABCD123",
					name = "Visa Inc. USA-Foster City",
					terminalId = "ABCD1234"
				},
				localTransactionDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
				merchantCategoryCode = "6012",
				pointOfServiceData = new
				{
					environment = "moto",
					panEntryMode = "90",
					posConditionCode = "00"
				},
				recipientName = "John Smith",
				recipientPrimaryAccountNumber = "4957030420210496",
				retrievalReferenceNumber = "330000550000",
				senderPrimaryAccountNumber = "4957030005123304",
				senderAddress = "901 Metro Center Blvd",
				senderCity = "Foster City",
				senderCountryCode = "840",
				senderStateCode = "CA",
				systemsTraceAuditNumber = "451050",
				transactionCurrencyCode = "USD",
			};
			string timestamp = DateTime.UtcNow.ToString("o");
			string xPayToken = GenerateXPayToken(resourcePath, "", request.ToString(), apiKey, sharedSecret, timestamp);
   		    // Serialize the request body to JSON
			string jsonRequest = JsonConvert.SerializeObject(request);

   		    // Build the HTTP request
			string url = baseUrl + resourcePath + "?apikey=" + apiKey;
			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Add("X-PAY-TOKEN:", xPayToken);


			var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
			var response = await client.PostAsync(url, content);
   		     // Check if the request was successful
			if (response.IsSuccessStatusCode)
			{
        		// Deserialize the response body
				string responseBody = await response.Content.ReadAsStringAsync();
				var responseObject = JsonConvert.DeserializeObject(responseBody);

        		// Do something with the response
				Console.WriteLine("success! message : " + responseObject);
			}
			else
			{
      		  	// Log the error
				Console.WriteLine(response.StatusCode);
				Console.WriteLine(await response.Content.ReadAsStringAsync());
			}
		}

		public static string GenerateXPayToken(string resourcePath, string queryString, string requestBody, string apiKey, string sharedSecret, string timestamp)
		{
			var beforeHash = apiKey + timestamp + resourcePath + queryString + requestBody;
			var secretBytes = Encoding.UTF8.GetBytes(sharedSecret);
			var inputBytes = Encoding.UTF8.GetBytes(beforeHash);

			using (var hmac = new HMACSHA256(secretBytes))
			{
				var hashBytes = hmac.ComputeHash(inputBytes);
				var base64Hash = Convert.ToBase64String(hashBytes);

				var token = "xv2:" + timestamp + ":" + base64Hash;

				return token;
			}
		}
	}
}