CREATE PROCEDURE usp_instructor_editar(
	@InstructorId uniqueidentifier,
	@Nombre nvarchar(500),
	@Apellidos nvarchar(500),
	@Titulo nvarchar(100)
)
AS
	BEGIN

		UPDATE Instructor 
		SET
			Nombre = @Nombre,
			Apellidos = @Apellidos,
			Grado = @Titulo
		WHERE InstructorId = @InstructorId

	END