-- Script para atualizar o hash de senha no banco de dados
-- Substitua 'seu@email.com' pelo email do usuário e o hash pelo hash fornecido

UPDATE Users
SET PasswordHash = 'AQAAAAIAAYagAAAAELWh8tUgNMc+vcCBBge2mAqLFOB7VPQV65RxGMHpoQDxXrohfsEUkFO68KTnwQH3dQ=='
WHERE Email = 'seu@email.com';

-- Verificar se foi atualizado
SELECT Id, Username, Email, PasswordHash, Role, IsActive
FROM Users
WHERE Email = 'seu@email.com';

