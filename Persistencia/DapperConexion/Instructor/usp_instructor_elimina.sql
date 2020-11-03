--El caso de Eliminar es un poco distinto ya que no solo elimino de la tabla Instructor
--si no, tambien tengo que eliminra de la tabla CursoInstructor
--Al eliminar un instructor, tambien se tiene que eliminar las referencias que tenga con otras tablas
CREATE PROCEDURE usp_instructor_elimina(
	@InstructorId uniqueidentifier

)
AS 
	BEGIN
		--Elimino de la tabla CursoInstructor
		DELETE FROM CursoInstructor WHERE InstructorId = @InstructorId
		
		--Elimino de la tabla Instructor
		DELETE FROM Instructor WHERE InstructorId = @InstructorId
	END