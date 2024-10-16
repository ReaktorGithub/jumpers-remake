using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utils 
{
    public static string Wrap(string text, string color) {
        return "<color=" + color + ">" + text + "</color>";
    }

    public static GameObject FindChildByName(GameObject parent, string name) {
        if (parent.name == name) {
            return parent;
        }

        foreach (Transform child in parent.transform) {
            GameObject foundObject = FindChildByName(child.gameObject, name);
            if (foundObject != null) {
                return foundObject;
            }
        }

        return null;
    }

    public static IEnumerator StartPulse(SpriteRenderer spriteToAnimate, float pulseTime, float minAlpha) {
        float alpha = minAlpha;
        bool isIn = true;
        
        while (true) {
            while (isIn && alpha < 1f) {
                alpha += Time.deltaTime * pulseTime;
                spriteToAnimate.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            isIn = false;
            while (!isIn && alpha > 0.2f) {
                alpha -= Time.deltaTime * pulseTime;
                spriteToAnimate.color = new Color(1f, 1f, 1f, alpha / pulseTime);
                yield return null;
            }
            isIn = true;
        }
    }

    public static IEnumerator StartPulseImage(Image spriteToAnimate, float pulseTime, float minAlpha) {
        float alpha = minAlpha;
        bool isIn = true;
        
        while (true) {
            while (isIn && alpha < 1f) {
                alpha += Time.deltaTime * pulseTime;
                spriteToAnimate.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            isIn = false;
            while (!isIn && alpha > 0.2f) {
                alpha -= Time.deltaTime * pulseTime;
                spriteToAnimate.color = new Color(1f, 1f, 1f, alpha / pulseTime);
                yield return null;
            }
            isIn = true;
        }
    }

    public static IEnumerator MoveTo(GameObject objectToMove, Vector3 position, float time, Action callback = null) {
        while (Vector3.Distance(objectToMove.transform.localPosition, position) > 0.1) {
            objectToMove.transform.localPosition = Vector3.Lerp(
                objectToMove.transform.localPosition, position, time * Time.deltaTime
                );
            yield return null;
        }
        objectToMove.transform.localPosition = position;
        callback?.Invoke();
    }

    // Вернет true, если курсор находится в области GameObject

    public static bool RaycastPointer(GameObject objectToCompare) {
        PointerEventData eventData = new(EventSystem.current) {
            position = Input.mousePosition
        };
        List<RaycastResult> raysastResults = new();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        for (int i = 0; i < raysastResults.Count; i++) {
            if (raysastResults[i].gameObject == objectToCompare) {
                return true;
            }
        }
        return false;
    }

    public static bool GetRandomDecision() {
        System.Random random = new();
        int result = random.Next(0, 1);
        return result == 1;
    }

    public static int GetRandomInt(int min, int max) {
        System.Random random = new();
        return random.Next(min, max);
    }

    // Метод принимает список с кортежами вида (элемент любого типа, int)
    // Возвращает кортеж с наибольшим числом int.
    // Если есть несколько кортежей-претендентов, то возвращает случайный из них

    public static (T, int) GetMostValuableElement<T>(List<(T, int)> list) {
        if (list.Count == 0) {
            throw new ArgumentException("Список не может быть пустым.");
        }

        int maxInt = list.Max(x => x.Item2);

        // Фильтруем список, оставляя только элементы с максимальным int
        var candidates = list.Where(x => x.Item2 == maxInt).ToList();

        // Выбираем случайный элемент из кандидатов
        System.Random random = new();
        int randomIndex = random.Next(candidates.Count);

        return candidates[randomIndex];
    }

    public static List<T> GetRandomElements<T>(List<T> sourceList, int count) {
        if (sourceList.Count < count) {
            throw new ArgumentException("Количество элементов в исходном списке меньше, чем запрошенное количество случайных элементов.");
        }

        List<T> randomElements = new();
        randomElements.AddRange(sourceList.OrderBy(x => Guid.NewGuid()).Take(count));

        return randomElements;
    }

    public static T GetRandomElement<T>(List<T> sourceList) {
        if (sourceList.Count == 0) {
            throw new ArgumentException("Список не должен быть пустым.");
        }
        if (sourceList.Count == 1) {
            return sourceList[0];
        }
        System.Random random = new();
        int index = random.Next(0, sourceList.Count);
        return sourceList[index];
    }

    // Если число положительное, то добавляет +
    // Положительные числа окрашивает в зеленый, а отрицательные в красный

    public static (string, Color32) GetTextWithSymbolAndColor(int number) {
        string text = number > 0 ? "+" + number.ToString() : number.ToString();
        Color32 color = number > 0 ? new Color32(3,74,0,255) : new Color32(180,6,0,255);
        return (text, color);
    }
}
