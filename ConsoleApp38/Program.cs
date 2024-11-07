using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Клас студента
class Student
{
    public string Name { get; }
    public bool HasPriority { get; }

    public Student(string name, bool hasPriority = false)
    {
        Name = name;
        HasPriority = hasPriority;
    }
}

// Клас курсу
class Course
{
    public string Name { get; }
    public int Capacity { get; } = 3;
    private int currentEnrollment;
    private List<Student> students = new List<Student>();

    // Події для повідомлення студентів та викладачів
    public event EventHandler<string> StudentNotification;
    public event EventHandler<string> TeacherNotification;

    public Course(string name)
    {
        Name = name;
    }

    // Метод для запису студента на курс
    public void EnrollStudent(Student student)
    {
        if (currentEnrollment < Capacity || student.HasPriority)
        {
            students.Add(student);
            currentEnrollment++;
            StudentNotification?.Invoke(this, $"{student.Name}, ви успiшно записанi на курс \"{Name}\".");
            TeacherNotification?.Invoke(this, $"Записано студента {student.Name} на курс \"{Name}\". Кiлькiсть студентiв: {currentEnrollment}/{Capacity}");
        }
        else
        {
            StudentNotification?.Invoke(this, $"На курсi \"{Name}\" немає доступних мiсць для {student.Name}.");
        }
    }

    // Метод завершення курсу
    public void CompleteCourse()
    {
        Console.WriteLine($"Курс \"{Name}\" завершено.");

        // Відписка всіх студентів і викладачів від подій
        StudentNotification = null;
        TeacherNotification = null;

        students.Clear();
        currentEnrollment = 0;
    }
}

// Клас викладача
class Teacher
{
    public string Name { get; }

    public Teacher(string name)
    {
        Name = name;
    }

    // Метод для обробки повідомлень від курсу
    public void OnStudentEnrollment(object sender, string message)
    {
        Console.WriteLine($"{Name} отримав повiдомлення: {message}");
    }
}

// Основна програма
class Program
{
    static async Task Main()
    {
        Teacher teacher = new Teacher("Викладач Iванов");
        List<Course> courses = new List<Course>();

        Task teacherTask = Task.Run(() => TeacherNotifications());

        while (true)
        {
            Console.WriteLine("\nОберiть дiю:");
            Console.WriteLine("1. Додати новий курс");
            Console.WriteLine("2. Записати студента на курс");
            Console.WriteLine("3. Завершити курс");
            Console.WriteLine("4. Вийти");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddNewCourse(courses);
                    break;
                case "2":
                    EnrollStudentToCourse(courses, teacher);
                    break;
                case "3":
                    CompleteCourse(courses);
                    break;
                case "4":
                    Console.WriteLine("Завершення програми...");
                    foreach (var course in courses)
                    {
                        course.CompleteCourse();
                    }
                    return;
                default:
                    Console.WriteLine("Невiрний вибiр. Спробуйте знову.");
                    break;
            }
        }
    }

    // Метод для додавання нового курсу
    static void AddNewCourse(List<Course> courses)
    {
        Console.WriteLine("Введiть назву нового курсу:");
        string courseName = Console.ReadLine();
        Course newCourse = new Course(courseName);
        courses.Add(newCourse);
        Console.WriteLine($"Курс \"{courseName}\" додано.");
    }

    // Метод для запису студента на курс
    // Метод для записи студента на курс
    // Метод для записи студента на курс
    static void EnrollStudentToCourse(List<Course> courses, Teacher teacher)
    {
        if (courses.Count == 0)
        {
            Console.WriteLine("Немає доступних курсiв. Спочатку додайте курс.");
            return;
        }

        Console.WriteLine("Введiть iм'я студента:");
        string studentName = Console.ReadLine();

        Console.WriteLine("Чи має студент прiоритет? (так/ні):");
        bool hasPriority = Console.ReadLine().ToLower() == "так";

        Console.WriteLine("Оберiть курс для запису:");
        for (int i = 0; i < courses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {courses[i].Name}");
        }

        if (int.TryParse(Console.ReadLine(), out int courseIndex) && courseIndex > 0 && courseIndex <= courses.Count)
        {
            Course selectedCourse = courses[courseIndex - 1];

            // Убедиться, что подписка на событие добавлена только один раз
            selectedCourse.TeacherNotification -= teacher.OnStudentEnrollment;
            selectedCourse.TeacherNotification += teacher.OnStudentEnrollment;

            // Добавляем обработчик уведомлений для студента, если он еще не добавлен
            selectedCourse.StudentNotification -= DisplayStudentMessage;
            selectedCourse.StudentNotification += DisplayStudentMessage;

            Student student = new Student(studentName, hasPriority);
            selectedCourse.EnrollStudent(student);
        }
        else
        {
            Console.WriteLine("Невiрний вибiр курсу.");
        }
    }

    // Вспомогательный метод для отображения уведомлений студенту
    static void DisplayStudentMessage(object sender, string message)
    {
        Console.WriteLine(message);
    }



    // Метод завершення курсу
    static void CompleteCourse(List<Course> courses)
    {
        if (courses.Count == 0)
        {
            Console.WriteLine("Немає курсiв для завершення.");
            return;
        }

        Console.WriteLine("Оберiть курс для завершення:");
        for (int i = 0; i < courses.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {courses[i].Name}");
        }

        if (int.TryParse(Console.ReadLine(), out int courseIndex) && courseIndex > 0 && courseIndex <= courses.Count)
        {
            Course selectedCourse = courses[courseIndex - 1];
            selectedCourse.CompleteCourse();
            courses.Remove(selectedCourse);
            Console.WriteLine($"Курс \"{selectedCourse.Name}\" завершено та видалено.");
        }
        else
        {
            Console.WriteLine("Невiрний вибiр курсу.");
        }
    }

    // Метод для постійного відображення повідомлень для викладача
    static void TeacherNotifications()
    {
        Console.WriteLine("=== Повiдомлення для викладача ===");
        while (true)
        {
            Thread.Sleep(2000); // Очікування нових повідомлень
        }
    }
}
