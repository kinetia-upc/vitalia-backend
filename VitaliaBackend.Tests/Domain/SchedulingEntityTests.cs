using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Domain.Model.ValueObjects;

namespace VitaliaBackend.Tests.Domain;

public class AppointmentTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var scheduledAt = new DateTime(2026, 7, 15, 10, 0, 0);

        var appt = new Appointment(id, "APT-001", doctorId, patientId, "brc-001", scheduledAt, "General consultation");

        Assert.Equal(id, appt.Id);
        Assert.Equal("APT-001", appt.Code);
        Assert.Equal(doctorId, appt.DoctorId);
        Assert.Equal(patientId, appt.PatientId);
        Assert.Equal("brc-001", appt.BranchId);
        Assert.Equal(scheduledAt, appt.ScheduledAt);
        Assert.Equal("General consultation", appt.Reason);
        Assert.Equal(EAppointmentStatus.Scheduled, appt.Status);
        Assert.Equal(EPaymentStatus.Pending, appt.PaymentStatus);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var appt = new Appointment(Guid.Empty, "APT-002", Guid.NewGuid(), Guid.NewGuid(), "brc-001", DateTime.Now, "Reason");

        Assert.NotEqual(Guid.Empty, appt.Id);
    }

    [Fact]
    public void Constructor_WithStatus_SetsProvidedStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-003", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason",
            EAppointmentStatus.Confirmed, EPaymentStatus.Paid);

        Assert.Equal(EAppointmentStatus.Confirmed, appt.Status);
        Assert.Equal(EPaymentStatus.Paid, appt.PaymentStatus);
    }

    [Fact]
    public void StringConstructor_ParsesGuids()
    {
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var appt = new Appointment("APT-004", doctorId.ToString(), patientId.ToString(), "brc-001", DateTime.Now, "Test");

        Assert.Equal(doctorId, appt.DoctorId);
        Assert.Equal(patientId, appt.PatientId);
        Assert.NotEqual(Guid.Empty, appt.Id);
    }

    [Theory]
    [InlineData(EAppointmentStatus.Scheduled, true)]
    [InlineData(EAppointmentStatus.Confirmed, true)]
    [InlineData(EAppointmentStatus.Arrived, true)]
    [InlineData(EAppointmentStatus.InAttention, true)]
    [InlineData(EAppointmentStatus.Released, false)]
    [InlineData(EAppointmentStatus.Cancelled, false)]
    public void IsActive_ReturnsCorrectValue(EAppointmentStatus status, bool expected)
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-005", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason", status);

        Assert.Equal(expected, appt.IsActive);
    }

    [Fact]
    public void Confirm_SetsConfirmedStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-006", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason");

        appt.Confirm();

        Assert.Equal(EAppointmentStatus.Confirmed, appt.Status);
    }

    [Fact]
    public void MarkArrived_SetsArrivedStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-007", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason");

        appt.MarkArrived();

        Assert.Equal(EAppointmentStatus.Arrived, appt.Status);
    }

    [Theory]
    [InlineData(EAppointmentStatus.Scheduled)]
    [InlineData(EAppointmentStatus.Confirmed)]
    [InlineData(EAppointmentStatus.Arrived)]
    public void StartAttention_FromValidStatus_SetsInAttention(EAppointmentStatus fromStatus)
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-008", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason", fromStatus);

        appt.StartAttention();

        Assert.Equal(EAppointmentStatus.InAttention, appt.Status);
    }

    [Fact]
    public void StartAttention_FromCancelled_DoesNotChangeStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-009", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason", EAppointmentStatus.Cancelled);

        appt.StartAttention();

        Assert.Equal(EAppointmentStatus.Cancelled, appt.Status);
    }

    [Fact]
    public void Release_FromInAttention_SetsReleased()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-010", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason", EAppointmentStatus.InAttention);

        appt.Release();

        Assert.Equal(EAppointmentStatus.Released, appt.Status);
    }

    [Fact]
    public void Release_FromArrived_SetsReleased()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-011", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason", EAppointmentStatus.Arrived);

        appt.Release();

        Assert.Equal(EAppointmentStatus.Released, appt.Status);
    }

    [Fact]
    public void Release_FromScheduled_DoesNotChangeStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-012", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason", EAppointmentStatus.Scheduled);

        appt.Release();

        Assert.Equal(EAppointmentStatus.Scheduled, appt.Status);
    }

    [Fact]
    public void Cancel_SetsCancelledStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-013", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason", EAppointmentStatus.Confirmed);

        appt.Cancel();

        Assert.Equal(EAppointmentStatus.Cancelled, appt.Status);
    }

    [Fact]
    public void ChangeStatus_SetsProvidedStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-014", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason");

        appt.ChangeStatus(EAppointmentStatus.InAttention);

        Assert.Equal(EAppointmentStatus.InAttention, appt.Status);
    }

    [Fact]
    public void MarkAsPaid_SetsPaidPaymentStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-015", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason");

        appt.MarkAsPaid();

        Assert.Equal(EPaymentStatus.Paid, appt.PaymentStatus);
    }

    [Fact]
    public void ChangePaymentStatus_SetsProvidedStatus()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-016", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Reason");

        appt.ChangePaymentStatus(EPaymentStatus.Refunded);

        Assert.Equal(EPaymentStatus.Refunded, appt.PaymentStatus);
    }

    [Fact]
    public void Reschedule_UpdatesFieldsAndResetsToScheduled()
    {
        var newDoctor = Guid.NewGuid();
        var newPatient = Guid.NewGuid();
        var newDate = new DateTime(2026, 8, 1, 14, 0, 0);

        var appt = new Appointment(
            Guid.NewGuid(), "APT-017", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Old reason", EAppointmentStatus.InAttention);

        appt.Reschedule(newDoctor, newPatient, "brc-002", newDate, "New reason");

        Assert.Equal(newDoctor, appt.DoctorId);
        Assert.Equal(newPatient, appt.PatientId);
        Assert.Equal("brc-002", appt.BranchId);
        Assert.Equal(newDate, appt.ScheduledAt);
        Assert.Equal("New reason", appt.Reason);
        Assert.Equal(EAppointmentStatus.Scheduled, appt.Status);
    }

    [Fact]
    public void Reschedule_NullReason_KeepsPreviousReason()
    {
        var appt = new Appointment(
            Guid.NewGuid(), "APT-018", Guid.NewGuid(), Guid.NewGuid(),
            "brc-001", DateTime.Now, "Original reason");

        appt.Reschedule(Guid.NewGuid(), Guid.NewGuid(), "brc-001", DateTime.Now, null);

        Assert.Equal("Original reason", appt.Reason);
    }
}

public class AvailabilitySlotTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var date = new DateOnly(2026, 7, 15);
        var start = new TimeOnly(10, 0);
        var end = new TimeOnly(11, 0);

        var slot = new AvailabilitySlot(id, "SLOT-001", doctorId, "brc-001", date, start, end);

        Assert.Equal(id, slot.Id);
        Assert.Equal("SLOT-001", slot.Code);
        Assert.Equal(doctorId, slot.DoctorId);
        Assert.Equal("brc-001", slot.BranchId);
        Assert.Equal(date, slot.Date);
        Assert.Equal(start, slot.StartTime);
        Assert.Equal(end, slot.EndTime);
        Assert.Equal(EAvailabilitySlotStatus.Available, slot.Status);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var slot = new AvailabilitySlot(Guid.Empty, "SLOT-002", Guid.NewGuid(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0));

        Assert.NotEqual(Guid.Empty, slot.Id);
    }

    [Fact]
    public void Constructor_WithBookedStatus_SetsProvidedStatus()
    {
        var slot = new AvailabilitySlot(
            Guid.NewGuid(), "SLOT-003", Guid.NewGuid(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0),
            EAvailabilitySlotStatus.Booked);

        Assert.Equal(EAvailabilitySlotStatus.Booked, slot.Status);
    }

    [Fact]
    public void StringConstructor_ParsesGuid()
    {
        var doctorId = Guid.NewGuid();

        var slot = new AvailabilitySlot("SLOT-004", doctorId.ToString(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0));

        Assert.Equal(doctorId, slot.DoctorId);
        Assert.NotEqual(Guid.Empty, slot.Id);
    }

    [Fact]
    public void IsAvailable_ReturnsTrue_WhenAvailable()
    {
        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-005", Guid.NewGuid(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0),
            EAvailabilitySlotStatus.Available);

        Assert.True(slot.IsAvailable);
    }

    [Fact]
    public void IsAvailable_ReturnsFalse_WhenBooked()
    {
        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-006", Guid.NewGuid(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0),
            EAvailabilitySlotStatus.Booked);

        Assert.False(slot.IsAvailable);
    }

    [Fact]
    public void Matches_ReturnsTrue_WhenAllFieldsMatch()
    {
        var doctorId = Guid.NewGuid();
        var date = new DateOnly(2026, 7, 15);
        var start = new TimeOnly(10, 0);

        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-007", doctorId, "brc-001",
            date, start, new TimeOnly(11, 0));

        Assert.True(slot.Matches(doctorId, "brc-001", date, start));
    }

    [Fact]
    public void Matches_ReturnsFalse_WhenDoctorIdDiffers()
    {
        var date = new DateOnly(2026, 7, 15);
        var start = new TimeOnly(10, 0);

        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-008", Guid.NewGuid(), "brc-001",
            date, start, new TimeOnly(11, 0));

        Assert.False(slot.Matches(Guid.NewGuid(), "brc-001", date, start));
    }

    [Fact]
    public void Matches_ReturnsFalse_WhenBranchIdDiffers()
    {
        var doctorId = Guid.NewGuid();
        var date = new DateOnly(2026, 7, 15);
        var start = new TimeOnly(10, 0);

        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-009", doctorId, "brc-001",
            date, start, new TimeOnly(11, 0));

        Assert.False(slot.Matches(doctorId, "brc-002", date, start));
    }

    [Fact]
    public void Matches_StringOverload_ReturnsCorrectValue()
    {
        var doctorId = Guid.NewGuid();
        var date = new DateOnly(2026, 7, 15);
        var start = new TimeOnly(10, 0);

        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-010", doctorId, "brc-001",
            date, start, new TimeOnly(11, 0));

        Assert.True(slot.Matches(doctorId.ToString(), "brc-001", date, start));
    }

    [Fact]
    public void Book_SetsBookedStatus()
    {
        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-011", Guid.NewGuid(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0));

        slot.Book();

        Assert.Equal(EAvailabilitySlotStatus.Booked, slot.Status);
    }

    [Fact]
    public void Release_SetsAvailableStatus()
    {
        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-012", Guid.NewGuid(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0),
            EAvailabilitySlotStatus.Booked);

        slot.Release();

        Assert.Equal(EAvailabilitySlotStatus.Available, slot.Status);
    }

    [Fact]
    public void ChangeStatus_SetsProvidedStatus()
    {
        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-013", Guid.NewGuid(), "brc-001",
            DateOnly.FromDateTime(DateTime.Now), new TimeOnly(10, 0), new TimeOnly(11, 0));

        slot.ChangeStatus(EAvailabilitySlotStatus.Booked);

        Assert.Equal(EAvailabilitySlotStatus.Booked, slot.Status);
    }

    [Fact]
    public void Reschedule_UpdatesAllFields()
    {
        var newDoctor = Guid.NewGuid();
        var newDate = new DateOnly(2026, 8, 1);
        var newStart = new TimeOnly(14, 0);
        var newEnd = new TimeOnly(15, 0);

        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-014", Guid.NewGuid(), "brc-001",
            new DateOnly(2026, 7, 15), new TimeOnly(10, 0), new TimeOnly(11, 0));

        slot.Reschedule(newDoctor, "brc-002", newDate, newStart, newEnd);

        Assert.Equal(newDoctor, slot.DoctorId);
        Assert.Equal("brc-002", slot.BranchId);
        Assert.Equal(newDate, slot.Date);
        Assert.Equal(newStart, slot.StartTime);
        Assert.Equal(newEnd, slot.EndTime);
    }

    [Fact]
    public void Reschedule_StringOverload_Works()
    {
        var newDoctor = Guid.NewGuid();

        var slot = new AvailabilitySlot(Guid.NewGuid(), "SLOT-015", Guid.NewGuid(), "brc-001",
            new DateOnly(2026, 7, 15), new TimeOnly(10, 0), new TimeOnly(11, 0));

        slot.Reschedule(newDoctor.ToString(), "brc-001", new DateOnly(2026, 8, 1), new TimeOnly(14, 0), new TimeOnly(15, 0));

        Assert.Equal(newDoctor, slot.DoctorId);
    }
}
