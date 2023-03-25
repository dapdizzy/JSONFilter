using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSONFilterTest
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void TestDoesSatisfyCriteria()
        {
            var json = @"
{
    ""items"": [
        {
            ""id"": 10461944,
            ""supplierId"": 11126227,
            ""supplierType"": ""THIRD_PARTY"",
            ""ssku"": null,
            ""msku"": 100869202058,
            ""count"": 1,
            ""price"": 89990.0,
            ""vat"": ""VAT_20_120"",
            ""warehouseId"": 51810
        }
    ],
    ""multiorderId"": null,
    ""businessRecipient"": null,
    ""deliveryPrice"": 799.0,
    ""userId"": null,
    ""outletCode"": null,
    ""personalBuyerAddressId"": ""baabccc9085d464a863ba29ce472c87e"",
    ""deliveryDate"": ""2023-03-15T00:00:00+03:00"",
    ""eventType"": null,
    ""deliveryType"": ""DELIVERY"",
    ""deliveryRegionId"": 213,
    ""fulfilment"": false,
    ""substatus"": ""USER_WANTS_TO_CHANGE_DELIVERY_DATE"",
    ""status"": ""CANCELLED"",
    ""contractId"": null,
    ""createdAt"": ""2023-03-13T09:42:24.942+03:00"",
    ""eventId"": 309207493,
    ""deliveryServiceId"": 99,
    ""personalShopAddressId"": ""baabccc9085d464a863ba29ce472c87e"",
    ""updatedAt"": ""2023-03-13T09:46:39.368+03:00"",
    ""buyerCurrency"": ""RUR"",
    ""id"": 33479146
}
";
            var jsonCriteria = @"
[
    {
        ""fieldPath"": ""$.deliveryType"",
        ""op"": ""Contains"",
        ""value"": [
            ""DELIVERY"",
            ""Something"",
            ""Else""
        ]
    }
]
";
            var result = JSONFilter.Filter.DoesSatisfyCriteria(json, jsonCriteria, out var errorMessage);
            Assert.AreEqual(true, result);
            Assert.AreEqual(null, errorMessage);
        }
    }
}