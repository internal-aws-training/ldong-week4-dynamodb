using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbLocalSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        // GET /sample/init
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

    }
}