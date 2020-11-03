CREATE PROCEDURE usp_obtener_instructor_por_id(
	@Id uniqueidentifier
)
AS
	BEGIN
		SELECT InstructorId, Nombre, Apellidos, Grado AS Titulo 
		FROM Instructor 
		WHERE InstructorId = @Id
	END