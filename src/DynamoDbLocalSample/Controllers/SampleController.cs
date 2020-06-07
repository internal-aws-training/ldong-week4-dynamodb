using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbLocalSample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class SampleController : ControllerBase
    {
        private const string TableName = "Project_ldong_SampleData";

        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly ILogger<SampleController> _logger;

        public SampleController(IAmazonDynamoDB amazonDynamoDb, ILogger<SampleController> logger)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _logger = logger;
        }

        // GET api/sample/init
        [HttpGet]
        [Route("init")]
        public async Task Initialise()
        {
            var request = new ListTablesRequest
            {
                Limit = 10
            };

            var response = await _amazonDynamoDb.ListTablesAsync(request);

            var results = response.TableNames;

            if (!results.Contains(TableName))
            {
                var createRequest = new CreateTableRequest
                {
                    TableName = TableName,
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "N"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType = "HASH"  //Partition key
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 2,
                        WriteCapacityUnits = 2
                    }
                };

                await _amazonDynamoDb.CreateTableAsync(createRequest);
            }
            else
            {
                _logger.LogWarning($"{TableName} exists already");
            }
        }

        // GET api/sample/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { N = id.ToString() } } }
            };

            var response = await _amazonDynamoDb.GetItemAsync(request);

            if (!response.IsItemSet)
            {
                return NotFound();
            }

            return response.Item["Title"].S;
        }

        // POST api/sample/
        [HttpPost]
        public async Task Post([FromBody] PostInput input)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = input.Id.ToString() }},
                    { "Title", new AttributeValue { S = input.Title }}
                }
            };

            await _amazonDynamoDb.PutItemAsync(request);
        }

        // DELETE api/sample/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { N = id.ToString() } } }
            };

            var response = await _amazonDynamoDb.DeleteItemAsync(request);

            return StatusCode((int)response.HttpStatusCode);
        }
    }
}