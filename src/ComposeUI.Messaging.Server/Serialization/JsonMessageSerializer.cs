﻿using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ComposeUI.Messaging.Core.Messages;

namespace ComposeUI.Messaging.Prototypes.Serialization;

/// <summary>
///     Serializes/deserializes messages to/from JSON
/// </summary>
public static class JsonMessageSerializer
{
    public static readonly JsonSerializerOptions Options =
        new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = {new JsonMessageConverter(), new JsonStringEnumConverter()},
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    /// <summary>
    ///     Serializes a message to UTF8-encoded JSON.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static byte[] SerializeMessage(Message message)
    {
        return JsonSerializer.SerializeToUtf8Bytes(message, Options);
    }

    /// <summary>
    ///     Deserializes a message from UTF8-encoded JSON
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static Message DeserializeMessage(ref ReadOnlySequence<byte> buffer)
    {
        var reader = new Utf8JsonReader(buffer);
        var message = JsonSerializer.Deserialize<Message>(ref reader, Options);
        buffer = buffer.Slice(reader.Position);
        return message;
    }

    private class JsonMessageConverter : JsonConverter<Message>
    {
        public override Message? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var innerReader = reader;
            if (innerReader.TokenType != JsonTokenType.StartObject) goto InvalidJson;
            while (innerReader.Read())
                switch (innerReader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if (innerReader.ValueTextEquals(TypePropertyNameBytes))
                        {
                            if (!innerReader.Read()) goto InvalidJson;
                            MessageType messageType;
                            switch (innerReader.TokenType)
                            {
                                case JsonTokenType.String:
                                    messageType = Enum.Parse<MessageType>(innerReader.GetString()!, ignoreCase: true);
                                    break;
                                case JsonTokenType.Number:
                                    messageType = (MessageType) innerReader.GetInt32();
                                    break;
                                default:
                                    goto InvalidJson;
                            }

                            var type = Message.ResolveMessageType(messageType);
                            return (Message) JsonSerializer.Deserialize(ref reader, type, options)!;
                        }

                        innerReader.Skip();
                        break;
                }

            InvalidJson:
            throw new JsonException();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Message);
        }

        public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }

        private static readonly byte[] TypePropertyNameBytes = Encoding.UTF8.GetBytes("type");
    }
}