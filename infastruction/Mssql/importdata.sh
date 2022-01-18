for i in {1..90};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost,1433 -U sa -P $MSSQL_SA_PASSWORD -d master -i setup.sql
    if [ $? -eq 0 ]
    then
        echo "setup.sql completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done