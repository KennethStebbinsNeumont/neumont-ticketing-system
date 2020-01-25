const defaultDatabases = ["admin", "config", "local"]

// Create mongo connection
let mongo = new Mongo();

// Remove old databases
let db = mongo.getDB("test");
let oldDbs = db.adminCommand('listDatabases').databases;
for (let i = 0; i < oldDbs.length; i++) {
    // Make sure we don't delete default databases
    if(!defaultDatabases.includes(oldDbs[i].name))
        oldDbs[i].dropDatabase();
}

// Insert test records
db = mongo.getDB("test");
db.tset.insert({ "name": "Hemlo World!", "value": "This is pretty neat-o!" });
