CREATE PROCEDURE usp_instructor_nuevo(
	@InstructorId uniqueidentifier,
	@Nombre nvarchar(500),
	@Apellidos nvarchar(500),
	@Titulo nvarchar(100)
)
AS
	BEGIN

		INSERT INTO Instructor(InstructorId, Nombre, Apellidos, Grado)
		VALUES(@InstructorId, @Nombre, @Apellidos, @Titulo)

	END