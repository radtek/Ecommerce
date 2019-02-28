# EptaEducacional

## Trabalhando com migrations
> Criar uma migration
```
add-migration **NOME-MIGRATION**
```
> Revertendo uma migration
```
update-database **NOME-MIGRATION-ANTERIOR**
```
Podemos reverter antes de fazer um pull request da branch que estamos trabalhando, assim, 
caso tenhamos criado mais de uma migration será apenas necessário executar um **Add-Migration** antes de comitar.

> Removendo uma migration errada
```
Remove-migration
```
Esse comando deleta uma migration que ainda não foi feita o **Update-Database**, 
se você executar com update-database já executado nada ocorrerá, é necessário
executar depois de reverter uma migration.

> Atualizar o banco de dados
```
Update-Database -Verbose
```
Atualiza o banco com a última migration criada.

> Gerar Script por migrations
```
Script-Migration
```
Deve-se executar para criar uma nova migration como script de banco.
