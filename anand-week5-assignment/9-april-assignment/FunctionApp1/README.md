# FunctionApp1 (HTTP Trigger)

This Azure Functions app exposes a simple HTTP endpoint that adds two integers passed via query parameters.

## Endpoint

- Route: `/api/Function1`
- Method: `GET` or `POST`
- Query params: `x` and `y` (integers)

### Example

`http://localhost:<port>/api/Function1?x=12&y=34`

Response: `46`

## Validation

- If `x` or `y` is missing or not an integer, the function returns **HTTP 400** with a short error message.
