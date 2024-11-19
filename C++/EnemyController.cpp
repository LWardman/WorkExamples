#include "Controllers/EnemyController.h"

#include "BehaviorTree/BehaviorTree.h"
#include "BehaviorTree/BehaviorTreeComponent.h"
#include "BehaviorTree/BlackboardComponent.h"
#include "Kismet/GameplayStatics.h"

#include "Characters/Enemy.h"


AEnemyController::AEnemyController()
{
    BTC = CreateDefaultSubobject<UBehaviorTreeComponent>(TEXT("Behavior Tree Component"));
    BBC = CreateDefaultSubobject<UBlackboardComponent>(TEXT("Blackboard Component"));
}

void AEnemyController::OnPossess(APawn* InPawn)
{
    Super::OnPossess(InPawn);

    if (AEnemy* Enemy = Cast<AEnemy>(InPawn))
    {

        if (Enemy->BehaviorTree)
        {
            BBC->InitializeBlackboard(*Enemy->BehaviorTree->BlackboardAsset);

            TryFindPlayerAndInitializeInBlackboard();

            BBC->SetValueAsBool("IsDead", false);

            BTC->StartTree(*(Enemy->BehaviorTree));
        }
    }
}

void AEnemyController::TryFindPlayerAndInitializeInBlackboard()
{
    APawn* Player = UGameplayStatics::GetPlayerPawn(this, 0);

    if (Player)
    {
        BBC->SetValueAsObject("Player", Player);
    }
    else
    {
        GetWorld()->GetTimerManager().SetTimerForNextTick(FTimerDelegate::CreateWeakLambda(this, [this]()
        {
            TryFindPlayerAndInitializeInBlackboard();
        }));
    }
}

void AEnemyController::StopBehaviorTree()
{
    if (!BTC) return;
    
    BTC->StopTree();
}
