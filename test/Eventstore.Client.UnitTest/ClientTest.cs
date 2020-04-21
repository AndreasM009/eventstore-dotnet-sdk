using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using NUnit.Framework;

namespace Eventstore.Client.UnitTest
{
    public class ClientTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAdd()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // expected response from eventstore
            var entity = new EventstoreEntity<TestDataObject>
            {
                id = obj.Id.ToString(),
                version = 1,
                data = obj
            };

            // add testdata
            var resultTask = client.Add("teststore", obj.Id.ToString(), obj);
            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(JsonSerializer.Serialize(entity))
            };
            httpClient.RespondWith(respMessage);

            // validate result
            var result = resultTask.Result;
            Assert.AreEqual((int)HttpStatusCode.Created, result.StatusCode);
            Assert.AreEqual(1, result.Version);
            Assert.IsNotNull(result.Resource);
            Assert.AreEqual(obj.Id, result.Resource.Id);
            Assert.AreEqual(obj.Firstname, result.Resource.Firstname);
            Assert.AreEqual(obj.Lastname, result.Resource.Lastname);
        }

        [Test]
        public void TestAddFails()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // add testdata
            var resultTask = client.Add("teststore", obj.Id.ToString(), obj);
            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            httpClient.RespondWith(respMessage);

            var result = resultTask.Result;

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.IsNull(result.Resource);
        }

        [Test]
        public void TestSave()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // expected response from eventstore
            var entity = new EventstoreEntity<TestDataObject>
            {
                id = obj.Id.ToString(),
                version = 2,
                data = obj
            };

            // save data
            var resultTask = client.Save("teststore", obj.Id.ToString(), obj);

            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(entity))
            };
            httpClient.RespondWith(respMessage);

            // validate result
            var result = resultTask.Result;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(2, result.Version);
            Assert.IsNotNull(result.Resource);
            Assert.AreEqual(obj.Id, result.Resource.Id);
            Assert.AreEqual(obj.Firstname, result.Resource.Firstname);
            Assert.AreEqual(obj.Lastname, result.Resource.Lastname);
        }

        [Test]
        public void TestSaveConflict()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // save data
            var resultTask = client.Save("teststore", obj.Id.ToString(), obj);

            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Conflict
            };
            httpClient.RespondWith(respMessage);

            var result = resultTask.Result;

            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.IsNull(result.Resource);
        }

        [Test]
        public void TestGet()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // expected response from eventstore
            var entity = new EventstoreEntity<TestDataObject>
            {
                id = obj.Id.ToString(),
                version = 1,
                data = obj
            };

            // save data
            var resultTask = client.Get<TestDataObject>("teststore", obj.Id.ToString());

            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(entity))
            };
            httpClient.RespondWith(respMessage);

            var result = resultTask.Result;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Version);
            Assert.IsNotNull(result.Resource);
            Assert.AreEqual(obj.Id, result.Resource.Id);
            Assert.AreEqual(obj.Firstname, result.Resource.Firstname);
            Assert.AreEqual(obj.Lastname, result.Resource.Lastname);
        }

        [Test]
        public void TestGetNotFound()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // save data
            var resultTask = client.Get<TestDataObject>("teststore", obj.Id.ToString());

            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound
            };
            httpClient.RespondWith(respMessage);

            var result = resultTask.Result;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.IsNull(result.Resource);
        }

        [Test]
        public void TestGetByVersion()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // expected response from eventstore
            var entity = new EventstoreEntity<TestDataObject>
            {
                id = obj.Id.ToString(),
                version = 1,
                data = obj
            };

            // save data
            var resultTask = client.GetByVersion<TestDataObject>("teststore", obj.Id.ToString(), 1);

            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(entity))
            };
            httpClient.RespondWith(respMessage);

            var result = resultTask.Result;
            Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(1, result.Version);
            Assert.IsNotNull(result.Resource);
            Assert.AreEqual(obj.Id, result.Resource.Id);
            Assert.AreEqual(obj.Firstname, result.Resource.Firstname);
            Assert.AreEqual(obj.Lastname, result.Resource.Lastname);
        }

        [Test]
        public void TestGetByVersionNotFound()
        {
            var httpClient = new TestingHttpClient();

            var builder = new EventstoreClientBuilder().UseHttpClient(httpClient);
            var client = builder.Build();

            // test data
            var obj = new TestDataObject
            {
                Id = Guid.NewGuid(),
                Firstname = "Hello",
                Lastname = "World"
            };

            // save data
            var resultTask = client.GetByVersion<TestDataObject>("teststore", obj.Id.ToString(), 1);

            // create expected response
            var respMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound
            };
            httpClient.RespondWith(respMessage);

            var result = resultTask.Result;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.IsNull(result.Resource);
        }
    }
}