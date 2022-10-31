using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
	public List<Sprite> characters = new List<Sprite>(); //это список спрайтов, которые вы будете использовать в качестве фрагментов плитки.
	public GameObject tile; //Префаб игрового объекта tile будет префабом, созданным при установке доски.
	public int xSize, ySize; // размеры платы по осям X и Y.

	public GameObject[,] tiles; //массив для хранения тайлов на доске

	public bool IsShifting { get; set; } // сообщит игре, когда будет найдено совпадение и поле снова заполнится.

	void Start () {
		instance = GetComponent<BoardManager>();//устанавливает синглтон со ссылкой на файл BoardManager
		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
		CreateBoard(offset.x , offset.y );//Размеры ячейки берем из размера спрайта 

	}
	public IEnumerator FindNullTiles()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
				{
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}

	}
	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
	{
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < ySize; y++)
		{  //Прокрутите и найдите, сколько пробелов нужно сместить вниз.
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
			{ //Сохраните количество пробелов в целом числе с именем nullCount.	
				nullCount++;
			}
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++)
		{ //Снова зациклите, чтобы начать фактическое переключение.
			yield return new WaitForSeconds(shiftDelay);//Пауза на shiftDelayсекунды.
			GUIManager.instance.Score += 50;

			for (int k = 0; k < renders.Count - 1; k++)//Перебрать все SpriteRenderer в списке renders.
			{ 
				renders[k].sprite = renders[k + 1].sprite;//Меняйте местами каждый спрайт с предыдущим до тех пор, пока не будет достигнут конец и последний спрайт не будет установлен наnull
				renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
			}
		}
		IsShifting = false;
	}
	private Sprite GetNewSprite(int x, int y)
	{
		List<Sprite> possibleCharacters = new List<Sprite>();
		possibleCharacters.AddRange(characters);

		if (x > 0)
		{
			possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < xSize - 1)
		{
			possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0)
		{
			possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}

		return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
	}




	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xSize, ySize]; //В CreateBoard()2D - массив tilesинициализируется.
		float startX = transform.position.x;//Общедоступные начальные данные для получения доски
		float startY = transform.position.y;//Найдите начальные позиции для генерации доски.
		Sprite[] previousLeft = new Sprite[ySize];
		Sprite previousBelow = null;


		//Прокручивайте xSizeи ySizeвыявляйте newTileкаждую итерацию для получения сетки строк и столбцов.
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
				tiles[x, y] = newTile;
				newTile.transform.parent = transform; //Привяжите все плитки к вашему BoardManager, чтобы ваша иерархия оставалась чистой в редакторе.


				List<Sprite> possibleCharacters = new List<Sprite>(); //Создайте список возможных символов для этого спрайта.
				possibleCharacters.AddRange(characters); //Добавьте всех персонажей в список.

				possibleCharacters.Remove(previousLeft[y]); //Удалите символы, которые находятся слева и ниже текущего спрайта, из списка возможных символов.
				possibleCharacters.Remove(previousBelow);


				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];//Случайным образом выберите спрайт из тех, которые вы ранее перетащили
				newTile.GetComponent<SpriteRenderer>().sprite = newSprite; //Set the newly created tile's sprite to the randomly chosen sprite.



				previousLeft[y] = newSprite;
				previousBelow = newSprite;


			}
		}

	}
}
