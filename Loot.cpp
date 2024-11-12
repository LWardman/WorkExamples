#include "Actors/LootDrops/Loot.h"

#include "Components/BoxComponent.h"

#include "NiagaraFunctionLibrary.h"

ALoot::ALoot()
{
 	PrimaryActorTick.bCanEverTick = true;

	Mesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Mesh"));
	Mesh->SetSimulatePhysics(true);
	Mesh->BodyInstance.bLockXRotation = true;
	Mesh->BodyInstance.bLockYRotation = true;
	
	RootComponent = Mesh;

	BoxCollision = CreateDefaultSubobject<UBoxComponent>(TEXT("Box Collision"));
	BoxCollision->SetupAttachment(Mesh);
	BoxCollision->OnComponentBeginOverlap.AddDynamic(this, &ALoot::OnOverlapBegin);
}

void ALoot::BeginPlay()
{
	Super::BeginPlay();

	SpawnLootMarker();

	ApplyInitialImpulse(InitialDropSpeed);

	if (bHasLifespan)
	{
		GetWorld()->GetTimerManager().SetTimer(
			LifetimeHandle, 
			this, 
			&ALoot::CallDestroy, 
			Lifespan, 
			false);
	}
}

void ALoot::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void ALoot::OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor,
                        class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep,
                        const FHitResult& SweepResult)
{
	
}

void ALoot::ApplyInitialImpulse(float ImpulseStrength)
{
	if (Mesh)
	{
		FVector RandVector = FMath::VRand();			// Random Unit Vector
		RandVector.Z = FMath::Abs(RandVector.Z);		// Ensures the random vector pops upwards (northern hemisphere)
		Mesh->AddImpulse(RandVector * ImpulseStrength);	// Adds random upwards vector as an impulse
	}
}

void ALoot::SpawnLootMarker()
{
	if (GetWorld() && LootMarker && Mesh)
	{
		FVector SpawnLocation = GetActorTransform().TransformPosition(LootMarkerLocation);

		UNiagaraFunctionLibrary::SpawnSystemAttached
			( 
			LootMarker,
			RootComponent,
			FName("None"),
			SpawnLocation,
			GetActorRotation(),
			EAttachLocation::KeepWorldPosition,
			true, //unsure
			true, //unsure
			ENCPoolMethod::None,
			true //unsure
			);
	}
}

void ALoot::CallDestroy()
{
	Destroy();
}
