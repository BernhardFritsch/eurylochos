The project was build with VisualStudio 2019, open it with VisualStudio 2019 to compile it.
You find a prebuild version in ./eurylochos/publish

Prerequisites:
.NET CORE 3.1

HOWTO run:
run the programm from a console or powershell window, The programm needs an argument to a CSV-File, which has the same format as the example file: "./eurylochos/publish/msg_standorte_deutschland.csv"
example:
.\eurylochos.exe --file C:\Users\Default\Desktop\msg_standorte_deutschland.csv
You can run the prebuild version in ./eurylochos/publish with the provided example data by running:
.\eurylochos.exe --file .\msg_standorte_deutschland.csv

About the Programm:
The Programm builds a graph from the Data in the CSV, it then performs a depth first search on this graph to find the shortest hamiltonian circle.
Therefore it runs a depth first search, where every path represents a hamiltonian circle on the graph. If a circle is found which is shorter than the shortest circle found so far,
it will be saved as the shortest circle found otherwise it will be discarded.
Path may be discarded before reaching the state of being a circle if they are allready longer than the shortest circle found, or if they visited less than half of the
vertexes in the graph and are allready longer than the half of the shortest circle found.

Note:
For the provided example data in eurylochos/publis/msg_standorte_deutschland.csv with 21 vertexes the programm needed about 30 mins to complete the search on my computer.
This time will grow faster than exponentially if more vertexes are added.
The complexity for the worst-case remains O(n!).