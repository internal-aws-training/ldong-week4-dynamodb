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

## Run Sample Api

```bash
dotnet run src/DynamoDbLocalSample
```

Routes:

| name        | Method | Url                            |
| ----------- | ------ | ------------------------------ |
| init        | GET    | localhost:5000/api/sample/init |
| get item    | GET    | localhost:5000/api/sample/5    |
| put item    | POST   | localhost:5000/api/sample/     |
| delete item | DELETE | locahost:5000/api/sample/5     |

data structure:

```json
{
  "Id" : 1,
  "Title": "Some Text"
}
```
