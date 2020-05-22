# Payment Gateway

API responsible for validating requests, storing card information and forwarding payment requests and accepting payment responses to and from the acquiring bank.

### Prerequisites

* Visual Studio 2017
* .NET Core SDK 3.1

### Swagger

* Payment Gateway API: https://localhost:44386/swagger/index.html
* Acquirer Bank Fake API: https://localhost:44336/swagger/index.html

### Requests

## Make a Payment

# Request
curl -X POST "https://localhost:44386/api/PaymentProcessing/v1" -H "accept: */*" -H "Content-Type: application/json" -d "{\"cardNumber\":\"1234567812345678\",\"expiryMonth\":12,\"expiryYear\":2025,\"cvv\":123,\"amount\":12.34,\"currencyIsoCode\":\"GBP\"}"

# Response
{
  "paymentGatewayId": "52a9c1f9-0b80-42cc-a7e7-449a860e026c",
  "success": true
}

## Retrieve a Payment

# Request
curl -X GET "https://localhost:44386/api/PaymentRetrieval/v1?id=52a9c1f9-0b80-42cc-a7e7-449a860e026c" -H "accept: */*"

# Response
{
  "paymentGatewayId": "52a9c1f9-0b80-42cc-a7e7-449a860e026c",
  "bankTransactionId": "f5ed5aee-ab9e-4b05-901a-60169f2713b7",
  "paymentStatus": 1,
  "cardNumber": "v5I2xXL6IwCWOKMSUzWBfkqFAV9uofAiAXJdYuPK+x9nesVnd+cpdPvsergoV3YzvyIt3LRPaebXsBFQCKbvbXD9GJ/gD3m5OtCvQ9xO2pKICkB5RlfibZZgs5DSvlPGBnn65WPB8KGWFQlLYI2RuNTMPia6uegfqYMSFJ+hdiI=",
  "expiryMonth": 12,
  "expiryYear": 2025,
  "cvv": 123,
  "amount": 12.34,
  "currencyIsoCode": "GBP"
}

### Built With

* .NET Core SDK 3.1: 

### TODO

todo

