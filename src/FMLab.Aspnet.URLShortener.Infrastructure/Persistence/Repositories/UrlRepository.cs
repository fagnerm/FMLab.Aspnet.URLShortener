// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FMLab.Aspnet.URLShortener.Business.Entities;
using FMLab.Aspnet.URLShortener.Business.Repositories;
using FMLab.Aspnet.URLShortener.Business.ValueObjects;

namespace FMLab.Aspnet.URLShortener.Infrastructure.Persistence.Repositories;
public class UrlRepository : IUrlRepository
{
    private readonly IAmazonDynamoDB _db;
    private const string TABLE_NAME = "url_redirections";

    public UrlRepository(IAmazonDynamoDB db)
    {
        _db = db;
    }

    public async Task AddAsync(UrlRedirection url, CancellationToken cancellationToken)
    {
        await _db.PutItemAsync(new PutItemRequest
        {
            TableName = TABLE_NAME,
            Item = new Dictionary<string, AttributeValue>
            {
                { "hash", new AttributeValue { S = url.Hash } },
                { "target", new AttributeValue { S = url.Target.ToString() } },
                { "temporary", new AttributeValue { BOOL = url.TemporaryRedirection } }
            }
        }, cancellationToken);
    }

    public async Task Delete(UrlRedirection user)
    {
        await _db.DeleteItemAsync(new DeleteItemRequest
        {
            TableName = TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "hash", new AttributeValue { S = user.Hash } }
            }
        });
    }

    public async Task<UrlRedirection?> GetByHashAsync(string hash, CancellationToken cancellationToken)
    {
        var itemResponse = await _db.GetItemAsync(new GetItemRequest
        {
            TableName = TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "hash", new AttributeValue { S = hash } }
            }
        }, cancellationToken);

        if (!itemResponse.IsItemSet) return null;

        return new UrlRedirection(itemResponse.Item["hash"].S, new Url(itemResponse.Item["target"].S), itemResponse.Item["temporary"].BOOL);
    }

    public async Task<UrlRedirection> Update(UrlRedirection user)
    {
        var updateItem = await _db.UpdateItemAsync(new UpdateItemRequest
        {
            TableName = TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "hash", new AttributeValue { S = user.Hash } }
            },
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#T", "target" },
                { "#R", "temporary" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":t", new AttributeValue { S = user.Target.ToString() } },
                { ":r", new AttributeValue { BOOL = user.TemporaryRedirection } }
            },
            UpdateExpression = "SET #T = :t, #R = :r",
            ReturnValues = "ALL_NEW"
        });

        return new UrlRedirection(updateItem.Attributes["hash"].S, new Url(updateItem.Attributes["target"].S), updateItem.Attributes["temporary"].BOOL);
    }

}
