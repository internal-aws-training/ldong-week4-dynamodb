#!/usr/bin/env bash
set -eu
workspace=$(cd "$(dirname "$0")" && pwd -P)

{
    action="$1"
    tableName=Project_ldong
    case "$action" in
    "createTable")
        aws dynamodb create-table \
            --table-name "$tableName" \
            --attribute-definitions \
            AttributeName=ProjectName,AttributeType=S \
            AttributeName=ProjectType,AttributeType=S \
            --key-schema \
            AttributeName=ProjectName,KeyType=HASH \
            AttributeName=ProjectType,KeyType=RANGE \
            --provisioned-throughput \
            ReadCapacityUnits=10,WriteCapacityUnits=5
        ;;
    "describeTable")
        aws dynamodb describe-table --table-name "${2:-$tableName}"
        ;;
    "deleteTable")
        aws dynamodb delete-table --table-name "${2:-$tableName}"
        ;;
    "put")
        num=${2:-}
        aws dynamodb put-item \
            --table-name "$tableName" \
            --item \
            '{"ProjectName": {"S": "Demo'"$num"'"},"ProjectType": {"S": "Test'"$num"'"},"MemberName": {"S": "Jack'"$num"'"}, "StartDate": {"S": "2020-05-29T13:42:34Z"},"Binary":{"B":"dGhpcyB0ZXh0IGlzIGJhc2U2NC1lbmNvZGVk"},"IsOK":{"BOOL": true},"BFlags":{"BS": ["U3Vubnk=", "UmFpbnk=", "U25vd3k="]},"List":{"L": [ {"S": "Cookies"} , {"S": "Coffee"}, {"N": "3.14159"}]},"Map":{"M": {"Name": {"S": "Joe"}, "Age": {"N": "35"}}},"Num":{"N": "123.45"},"Nums":{"NS": ["42.2", "-19", "7.5", "3.14"]},"Null":{"NULL": true},"Strings":{"SS": ["Giraffe", "Hippo" ,"Zebra"]}}'
        ;;
    "get")
        aws dynamodb get-item --consistent-read \
            --table-name "$tableName" \
            --key '{"ProjectName": {"S": "Demo"},"ProjectType": {"S": "Test"}}'
        ;;
    "scan")
        aws dynamodb scan \
            --table-name "$tableName" \
            --filter-expression "contains(MemberName, :a)" \
            --expression-attribute-values '{":a":{"S":"Jack"}}' \
            --projection-expression "#ST, #AT" \
            --expression-attribute-names '{"#ST": "ProjectName","#AT":"ProjectType"}' \
            --page-size 4 \
            --max-item 2
        # --starting-token "$2"

        ;;
    "addIndex")
        aws dynamodb update-table \
            --table-name "$tableName" \
            --attribute-definitions AttributeName=MemberName,AttributeType=S \
            --global-secondary-index-updates \
            "[{\"Create\":{\"IndexName\": \"MemberName-index\",\"KeySchema\":[{\"AttributeName\":\"MemberName\",\"KeyType\":\"HASH\"}], \"ProvisionedThroughput\": {\"ReadCapacityUnits\": 5, \"WriteCapacityUnits\": 2      },\"Projection\":{\"ProjectionType\":\"ALL\"}}}]"
        ;;

    "queryIndex")
        aws dynamodb query \
            --table-name "$tableName" \
            --index-name MemberName-index \
            --key-condition-expression "MemberName = :name" \
            --expression-attribute-values '{":name":{"S":"Jack4"}}'
        ;;

    "backup")
        aws dynamodb create-backup --table-name "$tableName" \
            --backup-name "${tableName}_backUp"
        ;;

    "listBackup")
        aws dynamodb list-backups
        ;;

    "restore")
        aws dynamodb restore-table-from-backup \
            --target-table-name "${tableName}_restore" \
            --backup-arn "$2"
        ;;

    "enableTimeRecovery")
        aws dynamodb update-continuous-backups \
            --table-name "$tableName" \
            --point-in-time-recovery-specification PointInTimeRecoveryEnabled=True
        ;;

    "showTimeRecovery")
        aws dynamodb describe-continuous-backups \
            --table-name "$tableName"
        ;;

    "recoveryRestorable")
        aws dynamodb restore-table-to-point-in-time \
            --source-table-name "$tableName" \
            --target-table-name "${tableName}-restorable" \
            --use-latest-restorable-time
        ;;

    "recoveryRestorableWithTime")
        aws dynamodb restore-table-to-point-in-time \
            --source-table-name "$tableName" \
            --target-table-name "${tableName}-restorable-with-time" \
            --no-use-latest-restorable-time \
            --restore-date-time "$2"
        ;;
    *)
        echo "Usage, supported cmd: createTable, describeTable, deleteTable, addIndex, queryIndex, put, get, delete, scan, backup, listBackup, restore, enableTimeRecovery, showTimeRecovery, recoveryRestorable, recoveryRestorableWithTime"
        ;;

    esac
}
