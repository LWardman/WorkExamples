#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Loot.generated.h"

class UNiagaraSystem;
class UBoxComponent;

UCLASS()
class TPSTEMPLATE_API ALoot : public AActor
{
	GENERATED_BODY()
	
public:	
	ALoot();

protected:
	virtual void BeginPlay() override;

public:	
	virtual void Tick(float DeltaTime) override;

	UFUNCTION()
    virtual void OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor,
                        class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep,
                        const FHitResult& SweepResult);

	UPROPERTY(EditAnywhere)
	UStaticMeshComponent* Mesh;

	UPROPERTY(EditAnywhere, Category = "Visual Effects")
	UNiagaraSystem* LootMarker;

	UPROPERTY(EditAnywhere, Meta = (MakeEditWidget = true))
	FVector LootMarkerLocation;

	UPROPERTY(EditAnywhere)
	UBoxComponent* BoxCollision;

	UPROPERTY(EditAnywhere)
	float InitialDropSpeed = 800.f;

	void ApplyInitialImpulse(float ImpulseStrength);

	void SpawnLootMarker();

	UFUNCTION()
	void CallDestroy();		// Only used since Destroy cannot be bound to a timer, as it isnt a ufunction.


	bool bHasLifespan = false;

	float Lifespan = 30.f;

	FTimerHandle LifetimeHandle;
};
