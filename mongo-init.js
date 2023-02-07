db.createUser(
    {
        user: "root",
        pwd: "rootpassword123",
        roles: [
            {
                role: "readWrite",
                db: "tvinfoDB"
            }
        ]
    }
);