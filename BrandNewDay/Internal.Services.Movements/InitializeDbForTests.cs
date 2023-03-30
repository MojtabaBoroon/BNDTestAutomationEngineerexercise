using System;
using Internal.Services.Movements.Data.Contexts;

public class InitializeDbForTests
{
	private readonly MovementsDataContext _movementsDb

	public InitializeDbForTests(MovementsDataContext movementsDb)
	{
		_movementsDb = movementsDb;
	}
}
