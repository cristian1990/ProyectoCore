
CREATE PROCEDURE [dbo].[usp_obtener_curso_paginacion](
	--Parametros de entrada
	@NombreCurso nvarchar(500), --Parametro creado en PaginacionCurso.cs (opcional)
	@Ordenamiento nvarchar(500),
	@NumeroPagina int,
	@CantidadElementos int,
	--Parametros de salida
	@TotalRecords int OUTPUT,
	@TotalPaginas int OUTPUT
)AS
BEGIN --Inicia la transaccion

	SET NOCOUNT ON --No devuelva el numero de transacciones
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED --Para que mi consulta no este en primer lugar

	DECLARE @Inicio int --Delaro variables
	DECLARE @Fin int

	--LOGICA PARA PAGINACION
	IF @NumeroPagina = 1  --Si el numero de pagina es 1
		BEGIN
			SET @Inicio = (@NumeroPagina*@CantidadElementos) - @CantidadElementos
			SET @Fin = @NumeroPagina * @CantidadElementos
		END
	ELSE --Si el numero de pagina es distinto de 1
		BEGIN
			SET @Inicio = ( (@NumeroPagina*@CantidadElementos) - @CantidadElementos ) + 1 --10*10-10+1=11
			SET @Fin = @NumeroPagina * @CantidadElementos --10*10
		END
	-------------------

	--CREO UNA TABLA TEMPORAL
	--Aca se almacena la data filtrada antes de devolverla al cliente
	--Tendra 2 campos/columnas rowNumber y ID (del curso)
	CREATE TABLE #TMP(
		rowNumber int IDENTITY(1,1),
		ID uniqueidentifier
	)
	-------------------

	--LOGICA PARA BUSCAR EL CURSO CON FILTRO
	DECLARE @SQL nvarchar(max)
	SET @SQL = ' SELECT CursoId FROM Curso ' --Obtengo todos los IDs de la tabla Curso
	
	IF @NombreCurso IS NOT NULL --Si el cliente envio algo para filtrar
		BEGIN
		   --Busco el titulo del curso, de acuerdo a lo ingresado
			SET @SQL = @SQL + ' WHERE Titulo LIKE ''%' + @NombreCurso +'%''  '
		END

	IF @Ordenamiento IS NOT NULL --Si el cliente indico un orden
		BEGIN
			--Ordeno de acuerdo a lo indica el usuario
			SET @SQL = @SQL + ' ORDER BY  ' + @Ordenamiento
		END

	--Como se veria la consulta
	--SELECT CursoId FROM Curso WHERE Titulo LIKE '% ASP %' ORDER BY Titulo
	----------------------

	INSERT INTO #TMP(ID) --Almaceno la data en la tabla temporal, indico que solo la columna ID (Tendra los ID de cursos y un identificador)
	EXEC sp_executesql @SQL --Ejecutamos la Query, retorna data despues de ejecutarse

	SELECT @TotalRecords =Count(*) FROM #TMP --Obtengo el total de la cantidad de registros

	--LOGICA PARA OBTENER EL TOTAL DE PAGINAS
	--@TotalRecords : Cantidad total de registros
	--@CantidadElementos : Registros que quiero mostrar por cada pagina
	IF @TotalRecords > @CantidadElementos --Si la cantidad total de registros es menor a la cantidad de registros a mostrar x pagina
		BEGIN
			SET @TotalPaginas = @TotalRecords / @CantidadElementos --50/10 = 5 paginas | 51/10 = 5.1 paginas
			IF (@TotalRecords % @CantidadElementos) > 0 --Si obtiene un decimal
				BEGIN
					SET @TotalPaginas = @TotalPaginas + 1 --Agrega 1 a la cantidad de paginas
				END
		END
	ELSE --Si no, bastaria solo con 1 pagina
		BEGIN
			SET @TotalPaginas = 1
		END
	---------------------

	--OBTENGO LA DATA A RETORNAR USANDO LA TABLA TEMPORAL
	--La tabla temporal solo guarda los IDs, por eso es necesario hacer un JOIN con las demas tablas para obtener la informacion
		SELECT 
			c.CursoId,
			c.Titulo,
			c.Descripcion,
			c.FechaPublicacion,
			c.FotoPortada,
			c.FechaCreacion,
			p.PrecioActual,
			p.Promocion
		FROM #TMP t INNER JOIN dbo.Curso c 
						ON t.ID = c.CursoId
					LEFT JOIN Precio p 
						ON c.CursoId = p.CursoId
		 WHERE t.rowNumber >= @Inicio AND t.rowNumber <= @Fin		
END