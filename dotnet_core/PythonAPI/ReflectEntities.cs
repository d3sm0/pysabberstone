﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf.Collections;
using SabberStoneCore.Enums;
using SabberStoneCore.Model.Entities;

namespace SabberStonePython.API
{
    public partial class GameId
    {
        public GameId(int id)
        {
            value_ = id;
        }
    }

    public partial class Game
    {
        private static int _id_gen;

        public Game(SabberStoneCore.Model.Game game, int id = -1)
        {
            currentPlayer_ = new Controller(game.CurrentPlayer);
            currentOpponent_ = new Controller(game.CurrentOpponent);
            state_ = (Types.State)game.State;
            turn_ = game.Turn;

            if (id < 0)
            {
                id = _id_gen++;;

                ManagedObjects.Games.Add(id, game);
                ManagedObjects.InitialGames.Add(id, game.Clone());
                //ManagedObjects.InitialGameAPIs.Add(id, this);
                ManagedObjects.OptionBuffers.Add(id, new List<Option>(50));
            }
            else
            {
                ManagedObjects.Games[id] = game;
            }
           
            id_ = new GameId(id);
        }
    }

    public partial class Controller
    {
        public Controller(SabberStoneCore.Model.Entities.Controller controller)
        {
            id_ = controller.PlayerId;
            hero_ = new Hero(controller.Hero);
            boardZone_ = new BoardZone(controller.BoardZone);
            handZone_ = new HandZone(controller.HandZone);
            secretZone_ = new SecretZone(controller.SecretZone);
            deckZone_ = new DeckZone(controller.DeckZone);
            playState_ = (Types.PlayState)controller.PlayState;
            baseMana_ = controller.BaseMana;
            remainingMana_ = controller.RemainingMana;
            overloadLocked_ = controller.OverloadLocked;
            overloadOwed_ = controller.OverloadOwed;
        }
    }

    public partial class BoardZone
    {
        public BoardZone(SabberStoneCore.Model.Zones.BoardZone boardZone)
        {
            var minions = new RepeatedField<Minion>();
            var span = boardZone.GetSpan();
            for (int i = 0; i < span.Length; i++) 
                minions.Add(new Minion(span[i]));

            minions_ = minions;
        }
    }

    public partial class HandZone
    {
        public HandZone(SabberStoneCore.Model.Zones.HandZone zone)
        {
            var playables = new RepeatedField<Playable>();
            var span = zone.GetSpan();
            for (int i = 0; i < span.Length; i++)
                playables.Add(new Playable(span[i], true));

            entities_ = playables;
        }
    }

    public partial class SecretZone
    {
        public SecretZone(SabberStoneCore.Model.Zones.SecretZone zone)
        {
            var playables = new RepeatedField<Playable>();
            var span = zone.GetSpan();
            for (int i = 0; i < span.Length; i++)
                playables.Add(new Playable(span[i], true));

            entities_ = playables;
        }
    }

    public partial class DeckZone
    {
        public DeckZone(SabberStoneCore.Model.Zones.DeckZone zone)
        {
            var playables = new RepeatedField<Playable>();
            var span = zone.GetSpan();
            for (int i = 0; i < span.Length; i++)
                playables.Add(new Playable(span[i], true));

            entities_ = playables;
        }
    }

    public partial class Minion
    {
        public Minion(SabberStoneCore.Model.Entities.Minion minion)
        {
            cardId_ = minion.Card.AssetId;
            atk_ = minion.AttackDamage;
            baseHealth_ = minion.BaseHealth;
            damage_ = minion.Damage;
            numAttacksThisTurn_ = minion.NumAttacksThisTurn;
            exhausted_ = minion.IsExhausted;
            attackableByRush_ = minion.AttackableByRush;
            charge_ = minion.HasCharge;
            windfury_ = minion.HasWindfury;
            lifesteal_ = minion.HasLifeSteal;
            poisonous_ = minion.Poisonous;
            stealth_ = minion.HasStealth;
            divineShield_ = minion.HasDivineShield;
            immune_ = minion.IsImmune;
            elusive_ = minion.CantBeTargetedBySpells;
            frozen_ = minion.IsFrozen;
            deathrattle_ = minion.HasDeathrattle;
            taunt_ = minion.HasTaunt;

            zonePosition_ = minion.ZonePosition;
            orderOfPlay_ = minion.OrderOfPlay;
        }
    }

    public partial class Playable
    {
        public Playable(IPlayable playable, bool hand)
        {
            cardId_ = playable.Card.AssetId;
            cost_ = playable.Cost;
            if (playable is Character c)
            {
                atk_ = c.AttackDamage;
                baseHealth_ = c.BaseHealth;
            }
            if (hand)
                ghostly_ = playable[SabberStoneCore.Enums.GameTag.GHOSTLY] == 1;
        }
    }

    public partial class Hero
    {
        public Hero(SabberStoneCore.Model.Entities.Hero hero)
        {
            cardClass_ = (int) hero.Card.Class;
            atk_ = hero.AttackDamage;
            baseHealth_ = hero.BaseHealth;
            damage_ = hero.Damage;
            numAttacksThisTurn_ = hero.NumAttacksThisTurn;
            armor_ = hero.Armor;
            exhausted_ = hero.IsExhausted;
            stealth_ = hero.HasStealth;
            immune_ = hero.IsImmune;

            power_ = new HeroPower(hero.HeroPower);
            if (hero.Weapon != null)
                weapon_ = new Weapon(hero.Weapon);
        }
    }

    public partial class HeroPower
    {
        public HeroPower(SabberStoneCore.Model.Entities.HeroPower heroPower)
        {
            cardId_ = heroPower.Card.AssetId;
            cost_ = heroPower.Cost;
            exhausted_ = heroPower.IsExhausted;
        }
    }

    public partial class Weapon
    {
        public Weapon(SabberStoneCore.Model.Entities.Weapon weapon)
        {
            cardId_ = weapon.Card.AssetId;
            atk_ = weapon.AttackDamage;
            durability_ = weapon.Durability;
            windfury_ = weapon.IsWindfury;
            lifesteal_ = weapon.HasLifeSteal;
            poisonous_ = weapon.Poisonous;
            immune_ = weapon.IsImmune;
        }
    }

    public partial class Option
    {
        public const int HERO_POSITION = 0;
        public const int OP_HERO_POSITION = 8;

        public Option(int gameId, Types.PlayerTaskType type, 
            int sourcePosition = -1, int targetPosition = -1, int subOption = 0,
            IPlayable source = null, ICharacter target = null)
        {
            gameId_ = gameId;
            type_ = type;
            sourcePosition_ = sourcePosition;
            targetPosition_ = targetPosition;
            subOption_ = subOption;

            var sb = new StringBuilder();
            switch (type)
            {
                case Types.PlayerTaskType.EndTurn:
                    sb.Append($"[END_TURN]");
                    break;
                case Types.PlayerTaskType.MinionAttack:
                case Types.PlayerTaskType.HeroAttack:
                    sb.Append($"[ATTACK] {source} => {target}");
                    break;
                case Types.PlayerTaskType.HeroPower:
                    sb.Append($"[HEROPOWER]{source}");
                    if (target != null)
                        sb.Append($" => {target}");
                    break;
                case Types.PlayerTaskType.PlayCard:
                    sb.Append($"[PLAY_CARD] {source}");
                    if (target != null)
                        sb.Append($" => {target}");
                    if (source.Card.Type == CardType.MINION)
                        sb.Append($"(Pos {targetPosition})");
                    else if (source.Card.Type == CardType.SPELL)
                        isSpell_ = true;
                    if (subOption > 0)
                        sb.Append($"(Opt {subOption}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            print_ = sb.ToString();
        }

        public Option(int gameId, int choice, string cardName)
        {
            gameId_ = gameId;
            type_ = Types.PlayerTaskType.Choose;
            choice_ = choice;
            print_ = $"[CHOOSE] {cardName}";
        }

        public Option(SabberStoneCore.Tasks.PlayerTasks.PlayerTask playerTask, int gameId)
        {
            gameId_ = gameId;
            print_ = playerTask.ToString();

            switch (playerTask)
            {
                case SabberStoneCore.Tasks.PlayerTasks.ChooseTask chooseTask:
                    choice_ = chooseTask.Choices[0];
                    type_ = Types.PlayerTaskType.Choose;
                    return;
                case SabberStoneCore.Tasks.PlayerTasks.MinionAttackTask minionAttackTask:
                    sourcePosition_ = getFriendlyPosition(minionAttackTask.Source);
                    targetPosition_ = getEnemyPosition(minionAttackTask.Target);
                    break;
                case SabberStoneCore.Tasks.PlayerTasks.HeroAttackTask heroAttackTask:
                    targetPosition_ = getEnemyPosition(heroAttackTask.Target);
                    break;
                case SabberStoneCore.Tasks.PlayerTasks.HeroPowerTask heroPowerTask:
                    targetPosition_ = getPosition(heroPowerTask.Target, heroPowerTask.Controller.Id);
                    break;
                case SabberStoneCore.Tasks.PlayerTasks.PlayCardTask playCardTask:
                    subOption_ = playCardTask.ChooseOne;
                    sourcePosition_ = playCardTask.Source.ZonePosition;     // SourcePosition: 0-based zone position in hand
                    if (playCardTask.Source.Card.Type == CardType.MINION)
                        targetPosition_ = playCardTask.ZonePosition + 1;    // Target Position:
                                                                            // Minions: 1-based zone position in board
                    else if (playCardTask.Source.Card.Type == CardType.SPELL)
                        targetPosition_ = getPosition(playCardTask.Target, playCardTask.Controller.Id);
                    break;                                                  // Spells:  0    : Hero
                                                                            //          1-7  : Minions
                                                                            //          8    : Op Hero
                                                                            //          9-15 : Op Minions
            }
            type_ = (Types.PlayerTaskType)playerTask.PlayerTaskType;

            if (playerTask.HasSource) sourceId_ = playerTask.Source.Id;
            if (playerTask.HasTarget) targetId_ = playerTask.Target.Id;
        }

        public static int getPosition(ICharacter character, int controllerId)
        {
            if (character == null)
                return -1;

            if (character.Controller.Id != controllerId)
            {
                if (character.Card.Type == CardType.MINION)
                    return character.ZonePosition + 9;
                else
                    return OP_HERO_POSITION;   // 8 for the opponent's Hero
            }
            else
            {
                if (character.Card.Type == CardType.MINION)
                    return character.ZonePosition + 1;
                else
                    return HERO_POSITION;   // 0 for the player's Hero
            }
        }

        public static int getFriendlyPosition(IPlayable character)
        {
            if (character.Card.Type == CardType.MINION)
                return character.ZonePosition + 1;
            else
                return HERO_POSITION;
        }

        public static int getEnemyPosition(ICharacter character)
        {
            if (character.Card.Type == CardType.MINION)
                return character.ZonePosition + 9;
            else
                return OP_HERO_POSITION;
        }
    }

    public partial class Options
    {
        public Options(List<Option> pyOptions)
        {
            var options = new RepeatedField<Option>();
            options.AddRange(pyOptions);
            list_ = options;
            pyOptions.Clear();
        }

        public Options(List<SabberStoneCore.Tasks.PlayerTasks.PlayerTask> playerTasks, int gameId)
        {
            var options = new RepeatedField<Option>();
            options.AddRange(playerTasks.Select(p => new Option(p, gameId)));
            list_ = options;
        }
    }

    public partial class Cards
    {
        public Cards(IEnumerable<SabberStoneCore.Model.Card> allCards)
        {
            cards_ = new MapField<int, Card>();

            foreach (var card in allCards)
            {
                if (card.Name == null)
                    continue;

                cards_.Add(card.AssetId, new Card
                {
                    Id = card.AssetId,
                    Name = card.Name,
                    StringId = card.Id
                });
            }
        }
    }
}
