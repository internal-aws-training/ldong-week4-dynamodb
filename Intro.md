# DynamoDb Local Sample

## Setup Dynamo Local

[DynamoDB Usage Notes](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.UsageNotes.html)

Run denamoDB using docker image

``` bash
docker run -p 8000:8000 amazon/dynamodb-local
```

List tabels

```bash
aws dynamodb list-tables --endpoint-url http://localhost:8000
```
