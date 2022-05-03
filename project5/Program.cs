using Implementation;

var distanceMatrix = PhylipParser.FromFile("test.phy");
var tree = SaitouNei.ToNewickFormat(distanceMatrix);
tree.Print();
