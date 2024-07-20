# 🥋 EsPublico Kata Orders

## 📰 Description

This project is a programming kata that involves importing a list of online orders from a REST API and exporting it to both a database and a CSV file.

## 🔍 Requirements

- Programming Language: C#
- Database: Any relational database with JDBC

## 🤖 Technologies Used

- Dotnet 8.0
- C# 12
- Postgres
- Docker

## 📕 Context

This is a set of orders that have been manually collected historically, with no further review than the human who entered the data. The data exposed in the API is nothing more than a dump of paper records to a digital format, without any processing.

## 👀 Objective

- Import the list of online orders into a database.
- The order list is available in a (public) REST API. The application must query this API to get all the orders to import.
- Import all this data into a database; and upon completion, display a summary of the number of orders of each type according to various columns.
- Additionally, generate a file with the records sorted by order number.
- The field by which the resulting file must be sorted is orderId.
- The final summary should show the count for each type of the fields: Region, Country, Item Type, Sales Channel, Order Priority.

## 📦 REST API

- [Swagger](https://kata-espublicotech.g3stiona.com/apidoc/#/orders/getOrder)
- [Endpoint](https://kata-espublicotech.g3stiona.com/v1/orders?page=1&max-per-page=100)

### 📄 Data Format

```json
{
  "page": 1,
  "content": [
    {
      "uuid": "1858f59d-8884-41d7-b4fc-88cfbbf00c53",
      "id": "443368995",
      "region": "Sub-Saharan Africa",
      "country": "South Africa",
      "item_type": "Fruits",
      "sales_channel": "Offline",
      "priority": "M",
      "date": "7/27/2012",
      "ship_date": "7/28/2012",
      "units_sold": 1593,
      "unit_price": 9.33,
      "unit_cost": 6.92,
      "total_revenue": 14862.69,
      "total_cost": 11023.56,
      "total_profit": 3839.13,
      "links": {
        "self": "https://kata-espublicotech.g3stiona.com:443/v1/orders/1858f59d-8884-41d7-b4fc-88cfbbf00c53"
      }
    }
  ],
  "links": {
    "next": "https://kata-espublicotech.g3stiona.com:443/v1/orders?page=2&max-per-page=100",
    "self": "https://kata-espublicotech.g3stiona.com:443/v1/orders?page=1&max-per-page=100"
  }
}
```

## 📄 Exported File Format

The resulting file should be a CSV with the following columns:

- Order Priority
- Order ID
- Order Date
- Region
- Country
- Item Type
- Sales Channel
- Ship Date
- Units Sold
- Unit Price
- Unit Cost
- Total Revenue
- Total Cost
- Total Profit

> 💡 **Note**: Dates should be displayed in the format: "{day}/{month}/{year}", with day and month as two-digit numbers, and year as four digits. Example: 12/06/1986

## 🤔 Questions

If you have any questions regarding the kata, you can contact us via email.

## ✏️ Diagrams

- [Flow Diagram](./docs/espublica-orders-flow-diagram.svg)