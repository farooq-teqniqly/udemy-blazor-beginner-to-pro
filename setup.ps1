New-Item -ItemType Directory -Force -Path secrets; 

@{ "db_user"="admin"; "db_password"="password123"; "pgadmin_password"="admin_password" }.GetEnumerator() ` 
| ForEach-Object { Set-Content -Path "secrets/$($_.Key).txt" -Value $_.Value }