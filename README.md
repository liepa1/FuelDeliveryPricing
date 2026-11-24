# Fuel Delivery Pricing

## Requirements

- .NET 8 SDK
- JSON files for shipments and payments in the program’s working directory

## Run

Open a terminal in the directory containing this README and run:

```powershell
dotnet run
```

The program reads input json files from the current working directory and writes CSV files to the configured output folder (`output` by default).

Enter console values in this order:

1. Custom VAT rate for private clients, as a percentage.
2. Custom gasoline excise rate.
3. Bonus 1 start date.
4. Bonus 1 end date.

Dates accept these exact formats:

- `yyyy-MM-dd` (for example, `2024-03-31`)
- `dd/MM/yyyy` (for example, `31/03/2024`)

## Completed parts

- Task 1
- Task 2
- Bonus 1
- Bonus 2



## Configuration Inputs

The application reads filenames, prices, rates and other provided values from `appsettings.json`.

## AI usage

OpenAI Codex was used:
- Discuss project structure options and usual practices
- Discuss parsing and validation approaches for input data
- Assist with calculation methods for most optimal code

I reviewed the AI suggestions, adapted them to the project requirements according to my own understanding of project scope and best suited solutions and checked that the solution built. I am responsible for the final implementation and for understanding how the submitted code works.

