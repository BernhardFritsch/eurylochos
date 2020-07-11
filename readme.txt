The project was build with VisualStudio 2019, open it with VisualStudio 2019 to compile it.
You find a prebuild version in ./eurylochos/publish

Prerequisites:
.NET CORE 3.1

HOWTO run:
run the programm from a console or powershell window, The programm needs an argument to a CSV-File, which has the same format as the example ./eurylochos/publish/msg_standorte_deutschland.csv
example:
.\eurylochos.exe --file C:\Users\Default\Desktop\msg_standorte_deutschland.csv
You can run the prebuild version in ./eurylochos/publish with the provided example data by running:
.\eurylochos.exe --file .\msg_standorte_deutschland.csv

About the Programm:
The Programm builds a graph from the Data in the CSV, it then performs a depth first search on this graph to find the shortest hamiltonian circle.