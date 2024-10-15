using System.Collections.Generic;

public static class SurpriseGenerator
{
    public static ESurprise GenerateSurpriseType(ESurprise forceReturn = ESurprise.None) {
        if (forceReturn != ESurprise.None) {
            return forceReturn;
        }

        // Некоторые элементы указаны несколько раз, чтобы увеличить вероятность выпадения

        List<ESurprise> types = new() {
            ESurprise.Yellow,
            ESurprise.Booster,
            ESurprise.Green,
            ESurprise.Black,
            ESurprise.Red,
            ESurprise.Star,
            ESurprise.Booster,
            ESurprise.Lightning,
            ESurprise.InventoryEffect,
            ESurprise.Teleport,
            ESurprise.Bonus,
            ESurprise.Penalty,
            ESurprise.Mallow,
            ESurprise.Booster,
            ESurprise.InventoryEffect,
            ESurprise.Booster,
        };

        return Utils.GetRandomElement(types);
    }

    public static EControllableEffects GenerateSurpriseInventoryEffect(EControllableEffects forceReturn = EControllableEffects.None) {
        if (forceReturn != EControllableEffects.None) {
            return forceReturn;
        }

        List<EControllableEffects> effects = new() {
            EControllableEffects.Black,
            EControllableEffects.Green,
            EControllableEffects.Red,
            EControllableEffects.Star,
            EControllableEffects.Yellow,
        };

        return Utils.GetRandomElement(effects);
    }

    public static EBoosters GenerateSurpriseBooster(EBoosters forceReturn = EBoosters.None) {
        if (forceReturn != EBoosters.None) {
            return forceReturn;
        }

        // Некоторые элементы указаны несколько раз, чтобы увеличить вероятность выпадения

        List<EBoosters> boosters = new() {
            EBoosters.Blot,
            EBoosters.Boombaster,
            EBoosters.Flash,
            EBoosters.Shield,
            EBoosters.Lasso,
            EBoosters.Magnet,
            EBoosters.MagnetSuper,
            EBoosters.Lasso,
            EBoosters.Shield,
            EBoosters.ShieldIron,
            EBoosters.Stuck,
            EBoosters.Trap,
            EBoosters.Vacuum,
            EBoosters.VacuumNozzle, // todo VacuumNozzle и Trap появляются, начиная с 6-й трассы.
            EBoosters.Vampire,
            EBoosters.Magnet,
        };

        return Utils.GetRandomElement(boosters);
    }

    public static int GenerateSurpriseCoins(bool isPenalty, int forceReturn = 0) {
        if (forceReturn != 0) {
            return forceReturn;
        }

        List<int> coins = new() {
            20,30,40,50,60,70,80,90,100,150,200 // todo Цифры 150 и 200 появляются, начиная с 6-й трассы.
        };

        int result = Utils.GetRandomElement(coins);
        return isPenalty ? -result : result;
    }

    public static int GenerateEffectLevel(int forceReturn = 0) {
        if (forceReturn != 0) {
            return forceReturn;
        }

        return Utils.GetRandomInt(1,4);
    }
}
