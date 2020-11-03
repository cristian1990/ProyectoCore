CREATE PROCEDURE usp_Obtener_Instructores
AS
	BEGIN
		SELECT i.InstructorId, i.Nombre, i.Apellidos, i.Grado 
		FROM Instructor i
	END