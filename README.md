# Payment Gateway

API responsible for validating requests, storing card information and forwarding payment requests and accepting payment responses to and from the acquiring bank.

## Prerequisites

* Visual Studio 2017
* .NET Core SDK 3.1

## Swagger/API Client

* Payment Gateway API: https://localhost:44386/swagger/index.html
* Acquirer Bank Fake API: https://localhost:44336/swagger/index.html

## Authentication

It requires Basic Auth for all requests, username: admin password: admin

## Requests

### Make a Payment

#### Request
curl -X POST "https://localhost:44386/api/PaymentProcessing/v1" -H "accept: */*" -H "Content-Type: application/json" -d "{\"cardNumber\":\"1234567812345678\",\"expiryMonth\":12,\"expiryYear\":2025,\"cvv\":123,\"amount\":12.34,\"currencyIsoCode\":\"GBP\"}"

#### Response
{
  "paymentGatewayId": "52a9c1f9-0b80-42cc-a7e7-449a860e026c",
  "success": true
}

### Retrieve a Payment

##### Request
curl -X GET "https://localhost:44386/api/PaymentRetrieval/v1?id=52a9c1f9-0b80-42cc-a7e7-449a860e026c" -H "accept: */*"

##### Response
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

## Request Model Validation

* Card Number: lenght from 1 to 50 (as some cards have more/less than 16 digits)
* Expiry Month: from 1 to 12
* Expiry Year: from 2020 to 2100
* CVV: from 1 to 999
* Amount: from 0.01 to Double.MaxValue
* CurrencyIsoCode: currently a string of lenght 3 

## Data Storage

A In-memory database and EntityFrameworkCore was used as data storage, which means that once the application is deployed or restarted any existing data would be erased.

## Logging Dashboard

Logs are being sent to elmah.io and there's a dashboard available. Credentials and URL are available via email.

## CI

CI script was built using Github Actions and are available here: https://github.com/thyagofalconi/paymentgateway/blob/master/.github/workflows/paymentgateway.yml

## Built With

* .NET Core SDK 3.1: https://dotnet.microsoft.com/download/dotnet-core/3.1
* Swagger: https://swagger.io/
* elmah.io: https://elmah.io/
* EntiyFrameworkCore: https://docs.microsoft.com/en-us/ef/core/
* Refit: https://github.com/reactiveui/refit
* FluentAssertions: https://fluentassertions.com/
* NSubstitute: https://nsubstitute.github.io/
* Nunit: https://nunit.org/

## Assumptions & Notes

* I assumed that the acquirer bank would return a transaction guid and a boolean response indicating if the request was successful or not
* I created the Acquirer Bank Fake API in dotnet core for the simplicty and how fast it is to build a new api using this framework
* All unit tests were written without a setup, so it's easy to understand and read the intent of what is being tested
* Refit was used to send requests to the Fake API, it's a very simple way to use an API without having to write too much code
* I'm saving a payment request before and after talking to the acquirer bank

## TODO & Areas to Improve

* Implement ISO-4217 for currency codes
* Improve model validation for payment requests
* If the api is not able to register a successful response from the Acquirer Bank, a transaction will be left saved as pending, so an additional process would have to be in place to check these transactions that were left behind. Or implement a retry policy to try n number of times before giving up
* Improve credit card data encryption, at the moment a RSA encryption is encrypting the credit card long number, but it would have to be something like: https://stackoverflow.com/questions/22047107/is-it-safe-to-have-a-credit-card-model
* To run the integration tests you will need to give permission in the firewall (a window will prompt if you try to run it without this exception). This is happening because the test runs two apis and it's hosting one of them in a different port. So it would be nice to use Kestrel without having to add this firewall exception
* Add more integration tests
* Implement more complex authentication (at the moment there's basic auth), for example, OAuth2 with https://auth0.com/
* Store keys as secrets in Github
* Add docker build and publish workflow in Github
* Once API is deployed in AWS, start using Grafana for application metrics
* Reduce build time
