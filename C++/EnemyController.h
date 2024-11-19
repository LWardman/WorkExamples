#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "EnemyController.generated.h"

class UBehaviorTreeComponent;
class UBlackboardComponent;

UCLASS()
class TPSTEMPLATE_API AEnemyController : public AAIController
{
	GENERATED_BODY()

	AEnemyController();

	virtual void OnPossess(APawn* InPawn) override;

	UPROPERTY(transient)
	UBehaviorTreeComponent* BTC;

	UPROPERTY(transient)
	UBlackboardComponent* BBC;

	void TryFindPlayerAndInitializeInBlackboard();

public:
	
	void StopBehaviorTree();
	
};
