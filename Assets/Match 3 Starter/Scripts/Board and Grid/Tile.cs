using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using System.Linq;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	private bool matchFound = false;

	[SerializeField] private string spriteNumber = "BubleGum";
	private string nameSprite;

	public static string kringe;


	
	
	void Awake() {
		render = GetComponent<SpriteRenderer>();
    }
	private void Start()
	{
    }	
    //сообщает игре, что этот фрагмент плитки был выбран, изменяет цвет плитки и воспроизводит звуковой эффект выбора. 
    private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}
	//возвращает спрайту его первоначальный цвет и сообщает игре, что в данный момент объект не выбран.
	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}
	void OnMouseDown()//Убедитесь, что игра разрешает выбор плитки.
                      //Бывают случаи, когда вы не хотите, чтобы игроки могли выбирать плитки, например, когда игра заканчивается или если плитка пуста.
	{
		
		if (render.sprite == null || BoardManager.instance.IsShifting)
		{
            var sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            Debug.Log(sprite.name);
            return;
		}

		if (isSelected)//определяет, следует ли выбрать или отменить выбор плитки. Если он уже выбран, отмените выбор.
		{ 
			Deselect();
		}
		else
		{
			if (previousSelected == null)
			{ // Проверьте, не выбрана ли уже другая плитка. Когда previousSelectedзначение null, это первое, поэтому выберите его.
				Select();
			}
			else
			{
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
				{ //Вызовите GetAllAdjacentTilesи проверьте, находится ли previousSelectedигровой объект в возвращаемом списке смежных тайлов.
					SwapSprite(previousSelected.render); //Поменять местами спрайт плитки.
					previousSelected.ClearAllMatches();

					previousSelected.Deselect();
					ClearAllMatches();


				}
				else
				{ //Если выбрана не первая плитка, снимите выделение со всех плиток.
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}

		}
	}
	public void SwapSprite(SpriteRenderer render2)
	{ // вызываемый render2в качестве параметра, который будет использоваться вместе с renderдля замены спрайтов.
		if (render.sprite == render2.sprite)
		{ //Проверка.Если они одинаковые, ничего не делайте, так как замена двух одинаковых спрайтов не имеет особого смысла.
			return;
		}

		Sprite tempSprite = render2.sprite; //Создайте объект tempSpriteдля хранения спрайта render2.
		render2.sprite = render.sprite; //Замените второй спрайт, установив его на первый.
		render.sprite = tempSprite; //Замените первый спрайт, установив его на второй (который был помещен в tempSprite.
		SFXManager.instance.PlaySFX(Clip.Swap); //Воспроизвести звуковой эффект.
		GUIManager.instance.MoveCounter--; //Произведет фактическую замену, как только вы выберете вторую плитку.
		

	}
	private GameObject GetAdjacent(Vector2 castDir)//звлечет один смежный тайл, отправив лучевую трансляцию в цель
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // Запускаем луч выстрела по позиции
		if (hit.collider != null)
		{
			return hit.collider.gameObject;//Если плитка найдена в этом направлении, вернуть ее GameObject
		}
		return null;
	}
	private List<GameObject> GetAllAdjacentTiles()//В методе GetAdjacent() для создания списка плиток 
	{
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
		{
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
        return adjacentTiles;
	}
	private List<GameObject> FindMatch(Vector2 castDir)
	{ //Этот метод принимает a Vector2 в качестве параметра, который будет направлением, в котором будут запущены все raycasts.
		List<GameObject> matchingTiles = new List<GameObject>(); ////Создайте новый список GameObjects для хранения всех совпадающих плиток.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); //Выпустите луч из плитки в castDirнаправлении.
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
		{ //Продолжайте запускать новые raycasts, пока либо ваш raycast ничего не достигнет, либо спрайт плитки не будет отличаться от возвращаемого спрайта объекта. Если оба условия соблюдены, вы считаете это совпадением и добавляете его в свой список.
			matchingTiles.Add(hit.collider.gameObject);
			nameSprite = render.gameObject.GetComponent<SpriteRenderer>().sprite.name;
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}
        return matchingTiles; // Вернуть список совпадающих спрайтов.

	}
	private void ClearMatch(Vector2[] paths) //Возьмите Vector2массив путей; это пути, по которым плитка будет транслироваться.
	{
		List<GameObject> matchingTiles = new List<GameObject>(); //Создайте список GameObject для хранения совпадений.
		for (int i = 0; i < paths.Length; i++) //Переберите список путей и добавьте в matchingTilesсписок все совпадения.
		{
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2) //Продолжайте, если было найдено совпадение с 2 или более плитками.
                                      //Вы можете задаться вопросом, почему здесь достаточно 2 совпадающих плиток, потому что третья совпадающая плитка является вашей начальной плиткой.
		{
			
			for (int i = 0; i < matchingTiles.Count; i++) // Переберите все совпадающие плитки и удалите их спрайты, установив null.
			{
                
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
			matchFound = true; //Установите matchFoundфлаг на true.
		}

    }

    public void ClearAllMatches()
	{
		if (render.sprite == null)
			return;

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
		{
			SpriteName();
			

			render.sprite = null;
			matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());

			SFXManager.instance.PlaySFX(Clip.Clear);

		}
	}
    void SpriteName() 
    {
	    nameSprite = render.gameObject.GetComponent<SpriteRenderer>().sprite.name;
	    kringe = nameSprite;
	    Debug.Log(nameSprite);
    }
    
	//2










}