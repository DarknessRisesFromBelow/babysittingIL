using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
   		    string userId = "ymacorpemail@gmail.com";
   		    string password = "Yma160207.";
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
   		                county = "San Mateo",
   		                state = "CA",
   		                zipCode = "94404"
   		            },
   		            idCode = "VMT200911026070",
   		            name = "Visa Inc. USA-Foster City",
   		            terminalId = "365539"
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
   		        senderAccountNumber = "4653459515756154",
   		        senderAddress = "901 Metro Center Blvd",
   		        senderCity = "Foster City",
   		        senderCountryCode = "840",
   		        senderName = "Mohammed Qasim",
   		        senderReference = "",
   		        senderStateCode = "CA",
   		        sourceOfFundsCode = "05",
   		        systemsTraceAuditNumber = "451050",
   		        transactionCurrencyCode = "USD",
   		        transactionIdentifier = "381228649430015"
   		    };

   		    // Serialize the request body to JSON
   		    string jsonRequest = JsonConvert.SerializeObject(request);

   		    // Calculate the HMAC signature
   		    string timestamp = DateTime.UtcNow.ToString("o");
   		    string preHashString = timestamp + resourcePath + jsonRequest;
   		    byte[] sharedSecretBytes = Encoding.UTF8.GetBytes(sharedSecret);
   		    byte[] preHashBytes = Encoding.UTF8.GetBytes(preHashString);
   		    byte[] hashBytes;
   		    using (var hasher = new System.Security.Cryptography.HMACSHA256(sharedSecretBytes))
   		    {
   		        hashBytes = hasher.ComputeHash(preHashBytes);
   		    }
   		    string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

   		    // Build the HTTP request
   		    string url = baseUrl + resourcePath;
   		    var client = new HttpClient();
   		    client.DefaultRequestHeaders.Accept.Clear();
   		    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
   		    client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey + ":" + password)));
   		    client.DefaultRequestHeaders.Add("X-Visa-Auth-Nonce", Guid.NewGuid().ToString("N"));
   		    client.DefaultRequestHeaders.Add("X-Visa-Auth-Timestamp", timestamp);
   		    client.DefaultRequestHeaders.Add("X-Visa-Auth-Signature", hash);

   		    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
   		    var response = await client.PostAsync(url, content);
   		     // Check if the request was successful
    		if (response.IsSuccessStatusCode)
   		 	{
        		// Deserialize the response body
        		string responseBody = await response.Content.ReadAsStringAsync();
        		var responseObject = JsonConvert.DeserializeObject(responseBody);

        		// Do something with the response
        		Console.WriteLine(responseObject);
    		}
    		else
    		{
      		  	// Log the error
        		Console.WriteLine(response.StatusCode);
        		Console.WriteLine(await response.Content.ReadAsStringAsync());
    		}
		}
	}
}